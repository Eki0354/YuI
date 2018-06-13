using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Room
{
    public class RoomLayout
    {
        public byte Building { get; }
        public byte Floor { get; }
        public RoomLayout(byte building, byte floor)
        {
            Building = building;
            Floor = floor;
        }
    }
}
