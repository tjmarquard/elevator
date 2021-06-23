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

        public Car()
        {
            FloorQueue = new List<int>();
            DirectionOfTravel = DirectionOfTravel.STAIONARY;
        }
    }
}
