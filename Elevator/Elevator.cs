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
                .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Car = new Car(numberOfFloors, Log.Logger);
            Floors = new List<Floor>();
            var floorNumbers = Enumerable.Range(1, numberOfFloors);
            foreach (var floorNumber in floorNumbers)
            {
                var floor = new Floor(floorNumber);
                Floors.Add(floor);
            }
        }

        public Car Car { get; set; }

        public List<Floor> Floors { get; set; }

        public int TopFloor
        {
            get
            {
                return Floors.Select(floor => floor.Number).Max();
            }
        }

        public bool QuitFlag { get; set; } = false;

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
                Car.DestinationFloorAndDirection();
                Car.SetNextFloor();
                await Car.MoveToNextFloor();
            }

            Car.IsInService = false;
        }

        private static string PushButtonPrompt()
        {
            Console.WriteLine("Which button in the elevator car was pushed?");
            return Console.ReadLine();
        }

        private void PushButtons()
        {
            var enteredValue = PushButtonPrompt();

            while (!CheckForValidButton(enteredValue))
            {
                Console.WriteLine("An invalid value was entered");
                enteredValue = PushButtonPrompt();
            }

            var floorNumber = GetEnteredFloorNumber(enteredValue);
            var directionOfTravel = GetEnteredDirectionOfTravel(enteredValue);

            var buttonPress = new ButtonPress()
            {
                FloorNumber = floorNumber,
                DirectionOfTravel = directionOfTravel,
            };

            Log.Information($"button press: {buttonPress.FloorNumber}{buttonPress.DirectionOfTravel}");

            Car.ButtonPresses.Add(buttonPress);

            ProcessButtonPresses();
        }
    }
}
