namespace Elevator
{
    using System;
    using System.Threading.Tasks;

    internal class Program
    {
        private static void Main()
        {
            var numberOfFloors = EnterHowManyFloors();
            var elevator = new Elevator(numberOfFloors);
            elevator.Run();

            while (elevator.Car.ButtonPresses.Count > 0)
            {
                Task.Delay(25);
            }
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

            var userEnteredValue = Console.ReadLine();

            if (userEnteredValue.ToUpperInvariant() == "Q")
            {
                Environment.Exit(0);
            }

            return userEnteredValue;
        }
    }
}
