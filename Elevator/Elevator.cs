using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Threading.Tasks;
using NLog;

namespace Elevator
{
    public class Elevator
    {
        public Car Car { get; set; }
        public List<Floor> Floors { get; set; }
        public Logger Logger { get; set; }

        public int TopFloor
        {
            get
            {
                return Floors.Select(floor => floor.Number).Max();
            }
        }
        public bool QuitFlag { get; set; } = false;

        private System.ComponentModel.BackgroundWorker backgroundWorker;

        public Elevator(int numberOfFloors)
        {
            Car = new Car(numberOfFloors);
            Floors = new List<Floor>();
            var floorNumbers = Enumerable.Range(1, numberOfFloors);
            foreach(var floorNumber in floorNumbers)
            {
                var floor = new Floor(floorNumber);
                Floors.Add(floor);
            }

            Logger = LogManager.GetLogger("*");
        }

        public void Run()
        {
            while (!QuitFlag)
            {
                PushButtons();
            }
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

            Car.ButtonPresses.Add(new ButtonPress 
            {
                FloorNumber = floorNumber,
                DirectionOfTravel = directionOfTravel 
            });

            ProcessButtonPresses();
        }

        private string PushButtonPrompt()
        {
            Console.WriteLine("Which button in the elevator car was pushed?");
            return Console.ReadLine();
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

        public int GetEnteredFloorNumber(string value)
        {
            var digits = Regex.Split(value, @"\D+");
            digits = digits.Where(digit => !string.IsNullOrEmpty(digit)).ToArray();

            Int32.TryParse(digits.FirstOrDefault(), out int floorNumber);
            return floorNumber;
        }

        public DirectionOfTravel GetEnteredDirectionOfTravel(string value)
        {
            var validChars = Regex.Replace(value.ToUpperInvariant(), @"[^ UDQ]", String.Empty);

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
                Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
                Logger.Info("hello");
                Car.DestinationFloorAndDirection();
                Car.SetNextFloor();
                await Car.MoveToNextFloor();
            }
            Car.IsInService = false;
        }
    }
}
