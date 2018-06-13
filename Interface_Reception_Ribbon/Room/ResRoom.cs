using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_Reception_Ribbon.Room
{
    public struct ResRoom
    {
        public Guest Guest { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public RoomType Type { get; set; }
        public long Price { get; set; }
        public byte Rooms { get; set; }
        public byte Persons { get; set; }
        public byte Nights { get; set; }
        public ResState State { get; set; }
        public DateTime Recorded { get; set; }
    }
}
