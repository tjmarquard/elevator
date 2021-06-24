using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator
{
    public class Floor
    {
        public int Number { get; set; }

        public Floor(int floorNumber)
        {
            Number = floorNumber;
        }
    }
}
