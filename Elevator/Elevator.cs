namespace Elevator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Serilog;

    public class Elevator
    {
        private readonly ILogger logger;

        public Elevator(int numberOfFloors)
        {
            logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/elevator.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Car = new Car(logger);
            FloorNumbers = Enumerable.Range(1, numberOfFloors).ToList();
        }

        public Car Car { get; set; }

        public List<int> FloorNumbers { get; set; }

        private bool QuitFlag { get; set; } = false;

        private int TopFloor
        {
            get
            {
                return FloorNumbers.Select(floorNumber => floorNumber).Max();
            }
        }

        public static int GetEnteredFloorNumber(string value)
        {
            var digits = Regex.Split(value, @"\D+");
            digits = digits.Where(digit => !string.IsNullOrEmpty(digit)).ToArray();

            if (int.TryParse(digits.FirstOrDefault(), out int floorNumber))
            {
                return floorNumber;
            }

            return 0;
        }

        public void Run()
        {
            logger.Information("started running the elevator");

            while (!QuitFlag)
            {
                PushButtons();
            }
        }

        public bool CheckForValidButton(string value)
        {
            var floorNumber = GetEnteredFloorNumber(value);

            if (floorNumber < 1 || floorNumber > TopFloor)
            {
                return false;
            }

            return true;
        }

        public DirectionOfTravel GetEnteredDirectionOfTravel(string value)
        {
            var validChars = Regex.Replace(value.ToUpperInvariant(), @"[^ UDQ]", string.Empty);

            if (validChars.Length == 0)
            {
                return DirectionOfTravel.NONE;
            }

            switch (validChars[0])
            {
                case 'U':
                    return DirectionOfTravel.UP;
                case 'D':
                    return DirectionOfTravel.DOWN;
                case 'Q':
                    QuitFlag = true;
                    return DirectionOfTravel.NONE;
                default:
                    return DirectionOfTravel.NONE;
            }
        }

        public async Task ProcessButtonPresses()
        {
            Car.IsInService = true;

            while (Car.ButtonPresses.Count > 0)
            {
                Car.SetDestinationFloor();
                Car.SetDirectionOfTravel();
                Car.SetNextFloor();
                await Car.MoveToNextFloor();
            }

            Car.IsInService = false;
        }

        private async Task<string> PushButtonPrompt()
        {
            Console.WriteLine("Which button in the elevator car was pushed?");

            var userEnteredValue = Console.ReadLine();

            if (userEnteredValue.ToUpperInvariant() == "Q")
            {
                QuitFlag = true;
                while (Car.ButtonPresses.Count != 0)
                {
                    await Task.Delay(25);
                }

                Environment.Exit(0);
            }

            return userEnteredValue;
        }

        private async void PushButtons()
        {
            var enteredValue = await PushButtonPrompt();

            while (!CheckForValidButton(enteredValue))
            {
                Console.WriteLine("An invalid value was entered");
                enteredValue = await PushButtonPrompt();
            }

            var floorNumber = GetEnteredFloorNumber(enteredValue);
            var directionOfTravel = GetEnteredDirectionOfTravel(enteredValue);

            if (QuitFlag)
            {
                return;
            }

            var buttonPress = new ButtonPress()
            {
                FloorNumber = floorNumber,
                DirectionOfTravel = directionOfTravel,
            };

            logger.Information($"button press: {buttonPress.FloorNumber} {buttonPress.DirectionOfTravel}");

            Car.ButtonPresses.Add(buttonPress);

            if (!Car.IsInService)
            {
                ProcessButtonPresses();
            }
        }
    }
}
