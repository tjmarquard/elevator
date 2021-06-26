namespace Elevator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Serilog;

    public class Car
    {
        private readonly ILogger logger;

        public Car(int numberOfFloors, ILogger logger)
        {
            this.logger = logger;
            ButtonPresses = new List<ButtonPress>();
            Floors = Enumerable.Range(1, numberOfFloors).ToList();
        }

        public DirectionOfTravel DirectionOfTravel { get; set; } = DirectionOfTravel.NONE;

        public bool IsMoving { get; set; } = false;

        public State State { get; set; } = State.STOPPED;

        public bool IsInService { get; set; } = false;

        public List<int> Floors { get; private set; }

        public int CurrentFloor { get; set; } = 1;

        public int DestinationFloor { get; set; } = 1;

        public int NextFloor { get; set; }

        public int MaxWeightLimit { get; } = 2000;

        public int Weight { get; set; }

        public bool IsUnderMaxWeightLimit
        {
            get => Weight <= MaxWeightLimit;
        }

        public bool IsOverMaxWeightLimit
        {
            get => Weight > MaxWeightLimit;
        }

        public List<ButtonPress> ButtonPresses { get; set; }

        public async Task MoveToNextFloor()
        {
            IsMoving = true;
            await Task.Delay(3 * 1000);
            IsMoving = false;
            CurrentFloor = NextFloor;
            if (CurrentFloor == DestinationFloor)
            {
                WaitAtCurrentFloor();
            }
            else
            {
                logger.Information($"pass floor: {CurrentFloor}");
            }
        }

        public void WaitAtCurrentFloor()
        {
            logger.Information($"stopped at floor: {CurrentFloor}");
            RemoveFloorFromQueue();
            System.Threading.Thread.Sleep(1 * 1000);
        }

        public void SetNextFloor()
        {
            if (DirectionOfTravel == DirectionOfTravel.UP)
            {
                NextFloor = CurrentFloor + 1;
            }
            else if (DirectionOfTravel == DirectionOfTravel.DOWN)
            {
                NextFloor = CurrentFloor - 1;
            }
            else
            {
                NextFloor = CurrentFloor;
            }
        }

        public void DestinationFloorAndDirection()
        {
            if (ButtonPresses.Count == 0)
            {
                return;
            }

            // Do I need to move up or down
            var nextFloorUp = ButtonPresses
                                .Where(buttonPress =>
                                    buttonPress.FloorNumber > CurrentFloor
                                    && (buttonPress.DirectionOfTravel == DirectionOfTravel.NONE
                                        || buttonPress.DirectionOfTravel == DirectionOfTravel.UP))
                                .OrderBy(floor => floor)
                                .FirstOrDefault();
            var nextFloorDown = ButtonPresses
                                .Where(buttonPress =>
                                    buttonPress.FloorNumber < CurrentFloor
                                    && (buttonPress.DirectionOfTravel == DirectionOfTravel.NONE
                                        || buttonPress.DirectionOfTravel == DirectionOfTravel.DOWN))
                                .OrderByDescending(floor => floor)
                                .FirstOrDefault();

            if (nextFloorUp != null && DirectionOfTravel == DirectionOfTravel.UP)
            {
                DestinationFloor = nextFloorUp.FloorNumber;
            }
            else if (nextFloorDown != null && DirectionOfTravel == DirectionOfTravel.DOWN)
            {
                DestinationFloor = nextFloorDown.FloorNumber;
            }
            else
            {
                var minDistance = ButtonPresses.Min(buttonPress => Math.Abs(buttonPress.FloorNumber - CurrentFloor));
                DestinationFloor = ButtonPresses.Select(buttonPress => buttonPress.FloorNumber)
                    .First(floorNumber => Math.Abs(floorNumber - CurrentFloor) == minDistance);
                if (DestinationFloor > CurrentFloor)
                {
                    DirectionOfTravel = DirectionOfTravel.UP;
                }
                else
                {
                    DirectionOfTravel = DirectionOfTravel.DOWN;
                }
            }
        }

        public void RemoveFloorFromQueue()
        {
            ButtonPresses.RemoveAll(buttonPress => buttonPress.FloorNumber == CurrentFloor && buttonPress.DirectionOfTravel == DirectionOfTravel);
            ButtonPresses.RemoveAll(buttonPress => buttonPress.FloorNumber == CurrentFloor && buttonPress.DirectionOfTravel == DirectionOfTravel.NONE);
        }
    }
}
