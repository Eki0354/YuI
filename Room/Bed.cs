using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Room
{
    public class Bed
    {
        public static BedItem DormBed = new BedItem(0, 200, 100);
        public static BedItem SingleBed = new BedItem(1, 200, 120);
        public static BedItem DoubleBed = new BedItem(2, 200, 150);
        public class BedItem
        {
            public byte ID { get; }
            public byte Length { get; }
            public byte Width { get; }
            public BedItem(byte id, byte length, byte width)
            {
                ID = id;
                Length = length;
                Width = width;
            }
        }
    }
}
