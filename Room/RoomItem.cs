using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Room
{
    public class RoomItem
    {
        public short Number { get; }
        public RoomType Type { get; }
        public RoomLayout Layout { get; }
        public bool IsAvailable { get; }
        public bool IsBooked { get; }
    }
}
