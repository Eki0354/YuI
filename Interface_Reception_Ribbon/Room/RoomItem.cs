using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_Reception_Ribbon.Room
{
    public enum RoomType
    {
        MIXED_6_DORM,
        MIXED_4_DORM,
        FEMALE_4_DORM,
        SINGLE_BUDGET,
        SINGLE_NORMAL,
        SINGLE_STANDARD,
        TWIN_NORMAL,
        TWIN_STANDARD,
        DOUBLE_NORMAL,
        DOUBLE_STANDARD,
        TRIPLE_NORMAL,
        TRIPLE_STANDARD,
        FAMILY
    }
    
    public struct RoomItem
    {
        public string ID { get; set; }
        public string Number { get; set; }
        public DateTime Birth { get; set; }
        public RoomType Type { get; set; }
    }
}
