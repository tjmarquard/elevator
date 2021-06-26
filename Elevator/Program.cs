namespace Elevator
{
    using System;

    internal class Program
    {
        private static void Main()
        {
            var numberOfFloors = EnterHowManyFloors();
            var elevator = new Elevator(numberOfFloors);
            elevator.Run();

            Console.WriteLine($"The elevator has served all floors and shutdown.");
        }

        private static int EnterHowManyFloors()
        {
            var userEnteredValue = PromptHowManyFloors();

            int numberOfFloors;

            while (!int.TryParse(userEnteredValue, out numberOfFloors))
            {
                Console.WriteLine("An invalid value was entered");
                userEnteredValue = PromptHowManyFloors();
            }

            return numberOfFloors;
        }

        private static string PromptHowManyFloors()
        {
            Console.WriteLine("How many floors does this elevator service?");

            return Console.ReadLine();
        }
    }
}
