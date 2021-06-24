using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator
{
    public class Car
    {
        public DirectionOfTravel DirectionOfTravel { get; set; }
        public List<int> FloorQueue { get; set; }
        public bool Moving { get; set; } = false;

        public List<int> Floors { get; private set; }
        public int CurrentFloor { get; set; } = 1;
        public int MaxWeightLimit { get; } = 2000;
        public int Weight { get; set; }
        public bool IsUnderMaxWeightLimit
        {
            get => Weight <= MaxWeightLimit;
        }

        public Car(int numberOfFloors)
        {
            FloorQueue = new List<int>();
            DirectionOfTravel = DirectionOfTravel.STAIONARY;
            Floors = Enumerable.Range(1, numberOfFloors).ToList();
        }

        public async void MovingToNextFloor()
        {
            await Task.Delay(3 * 1000);

            switch (DirectionOfTravel)
            {
                case DirectionOfTravel.DOWN:
                    CurrentFloor = CurrentFloor - 1;
                    break;
                case DirectionOfTravel.UP:
                    CurrentFloor = CurrentFloor + 1;
                    break;
                case DirectionOfTravel.STAIONARY:
                    break;
                default:
                    break;
            }
        }
    }
}
