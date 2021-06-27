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

        public Car(ILogger logger)
        {
            this.logger = logger;
            ButtonPresses = new List<ButtonPress>();
        }

        public DirectionOfTravel DirectionOfTravel { get; set; } = DirectionOfTravel.NONE;

        public bool IsMoving
        {
            get
            {
                return State == State.MOVING;
            }
        }

        public State State { get; set; } = State.STOPPED;

        public bool IsInService { get; set; } = false;

        public int CurrentFloor { get; set; } = 1;

        public int DestinationFloor { get; set; }

        public int NextFloor { get; set; }

        public int MaxWeightLimit { get; } = 2000;

        public int Weight { get; set; }

        public bool IsOverMaxWeightLimit
        {
            get => Weight > MaxWeightLimit;
        }

        public List<ButtonPress> ButtonPresses { get; set; }

        public async Task MoveToNextFloor()
        {
            if (DestinationFloor != 0 && DestinationFloor != CurrentFloor)
            {
                State = State.MOVING;
                await Task.Delay(3 * 1000);
                CurrentFloor = NextFloor;
                if (CurrentFloor == DestinationFloor)
                {
                    WaitAtFloor();
                }
                else
                {
                    PassByFloor();
                }
            }
        }

        public void WaitAtFloor()
        {
            logger.Information($"stopped at floor: {CurrentFloor}");
            SetDestinationFloor();
            SetDirectionOfTravel();
            RemoveFloorFromQueue();
            State = State.STOPPED;
            System.Threading.Thread.Sleep(1 * 1000);
        }

        public void PassByFloor()
        {
            logger.Information($"passed floor: {CurrentFloor}");
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

        public void SetDestinationFloor()
        {
            if (ButtonPresses.Count == 0)
            {
                DestinationFloor = 0;
                return;
            }

            var nextFloorUp = GetNextFloorUp();
            var nextFloorDown = GetNextFloorDown();

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
            }
        }

        public void SetDirectionOfTravel()
        {
            if (DestinationFloor == 0)
            {
                DirectionOfTravel = DirectionOfTravel.NONE;
            }
            else if (DestinationFloor > CurrentFloor)
            {
                DirectionOfTravel = DirectionOfTravel.UP;
            }
            else if (DestinationFloor < CurrentFloor)
            {
                DirectionOfTravel = DirectionOfTravel.DOWN;
            }
            else
            {
                DirectionOfTravel = DirectionOfTravel.NONE;
            }
        }

        public void RemoveFloorFromQueue()
        {
            ButtonPresses.RemoveAll(buttonPress => buttonPress.FloorNumber == CurrentFloor && buttonPress.DirectionOfTravel == DirectionOfTravel);
            ButtonPresses.RemoveAll(buttonPress => buttonPress.FloorNumber == CurrentFloor && buttonPress.DirectionOfTravel == DirectionOfTravel.NONE);
            if (DirectionOfTravel == DirectionOfTravel.NONE)
            {
                ButtonPresses.RemoveAll(buttonPress => buttonPress.FloorNumber == CurrentFloor);
            }
        }

        private ButtonPress GetNextFloorUp()
        {
            return ButtonPresses.Where(buttonPress =>
                                    buttonPress?.FloorNumber > CurrentFloor
                                    && (buttonPress?.DirectionOfTravel == DirectionOfTravel.NONE
                                        || buttonPress?.DirectionOfTravel == DirectionOfTravel.UP))
                                .OrderBy(buttonPress => buttonPress.FloorNumber)
                                .FirstOrDefault();
        }

        private ButtonPress GetNextFloorDown()
        {
            return ButtonPresses.Where(buttonPress =>
                                    buttonPress?.FloorNumber < CurrentFloor
                                    && (buttonPress?.DirectionOfTravel == DirectionOfTravel.NONE
                                        || buttonPress?.DirectionOfTravel == DirectionOfTravel.DOWN))
                                .OrderByDescending(buttonPress => buttonPress.FloorNumber)
                                .FirstOrDefault();
        }
    }
}
