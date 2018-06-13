using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Room
{
    public class RoomType
    {
        public string Name { get; }
        public RoomFacility Facility { get; }
        public byte Area { get; }
        public byte[] Beds { get; }
        public byte MaximumPersons { get; }
        public RoomType(string name, RoomFacility facility, byte area, byte[] beds, byte maxPersons)
        {
            Name = name;
            Facility = facility;
            Area = area;
            Beds = beds;
            MaximumPersons = maxPersons;
        }
    }
}
