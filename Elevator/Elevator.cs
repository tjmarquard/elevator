using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Elevator
{
    public class Elevator
    {
        public Car Car { get; set; }
        public List<Floor> Floors { get; set; }
        public int TopFloor
        {
            get
            {
                return Floors.Select(floor => floor.Number).Max();
            }
        }

        public Dictionary<int, DirectionOfTravel> Requests { get; set; }

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
        }

        public void Run()
        {

        }

        private void PushButtons()
        {
            var enteredValue = PushButtonPrompt();

            while (!CheckForValidButton(enteredValue))
            {
                Console.WriteLine("An invalid value was entered");
                enteredValue = PushButtonPrompt();
            }


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

        public string GetEnteredDirectionOfTravel(string value)
        {
            var validChars = Regex.Replace(value.ToUpperInvariant(), @"[^ UDQ]", String.Empty);

            if (validChars.Length > 0)
            {
                return validChars[0].ToString();
            }
            return "";
        }
    }
}
