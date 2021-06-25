using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator
{
    public class Car
    {
        public DirectionOfTravel DirectionOfTravel { get; set; } = DirectionOfTravel.NONE;
        public bool IsMoving { get; set; } = false;
        public State State { get; set; } = State.STOPPED;
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
        public ObservableCollection<(int, DirectionOfTravel)> FloorQueue { get; set; }

        public Car(int numberOfFloors)
        {
            FloorQueue = new ObservableCollection<(int, DirectionOfTravel)>();
            FloorQueue.CollectionChanged += HandleChange;

            Floors = Enumerable.Range(1, numberOfFloors).ToList();
        }

        private async void HandleChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            QuerySensor();

            SetNextFloor();

            await Task.Delay(3 * 1000);
            Console.WriteLine("added");
        }

        private void QuerySensor()
        {
            Console.WriteLine(DateTime.Now);
            Console.WriteLine($"Current Direction: {DirectionOfTravel}");
            if (IsMoving)
            {
                Console.WriteLine($"Next Floor: {DestinationFloor}");
            }
            else
            {
                Console.WriteLine($"Current Floor: {CurrentFloor}");
            }
            Console.WriteLine($"State: {State}");
            Console.WriteLine($"Over weight limit: {IsOverMaxWeightLimit}");
        }

        public void MoveToNextFloor()
        {
            QuerySensor();

            IsMoving = true;
            Task.Delay(3 * 1000);
            IsMoving = false;
            CurrentFloor = NextFloor;
            if (CurrentFloor == DestinationFloor)
            {
                WaitAtCurrentFloor();
            }

            QuerySensor();
        }

        public void WaitAtCurrentFloor()
        {
            RemoveFloorFromQueue();
            Task.Delay(1 * 1000);
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
            if (FloorQueue.Count == 0)
            {
                return;
            }

            //Do I need to move up or down
            var nextFloorUp = FloorQueue
                                .Where(floor => 
                                    floor.Item1 > CurrentFloor
                                    && (floor.Item2 == DirectionOfTravel.NONE
                                        || floor.Item2 == DirectionOfTravel.UP))
                                .OrderBy(floor => floor)
                                .FirstOrDefault();
            var nextFloorDown = FloorQueue
                                .Where(floor =>
                                    floor.Item1 < CurrentFloor
                                    && (floor.Item2 == DirectionOfTravel.NONE
                                        || floor.Item2 == DirectionOfTravel.DOWN))
                                .OrderByDescending(floor => floor)
                                .FirstOrDefault();


            if (DirectionOfTravel == DirectionOfTravel.UP)
            {
                DestinationFloor = nextFloorUp.Item1;
            }
            else if (DirectionOfTravel == DirectionOfTravel.DOWN)
            {
                DestinationFloor = nextFloorDown.Item1;
            }
            else if (DirectionOfTravel == DirectionOfTravel.NONE)
            {
                var minDistance = FloorQueue.Min(floor => Math.Abs(floor.Item1 - CurrentFloor));
                DestinationFloor = FloorQueue.Select(floor => floor.Item1)
                    .First(level => Math.Abs(level - CurrentFloor) == minDistance);
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
            FloorQueue.Remove((CurrentFloor, DirectionOfTravel));
            FloorQueue.Remove((CurrentFloor, DirectionOfTravel.NONE));
        }
    }
}
