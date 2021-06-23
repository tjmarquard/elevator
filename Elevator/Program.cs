using System;

namespace Elevator
{
    class Program
    {
        static void Main(string[] args)
        {
            var numberOfFloors = PromptHowManyFloors();

            int floors;

            while (!Int32.TryParse(numberOfFloors, out floors))
            {
                Console.WriteLine("An invalid value was entered");
                numberOfFloors = PromptHowManyFloors();
            }

            var car = new Car(floors);
            Console.WriteLine($"It serves {floors} floors.");


        }

        private static string PromptHowManyFloors()
        {
            Console.WriteLine("How many floors does this elevator service?");

            return Console.ReadLine();
        }
    }
}
