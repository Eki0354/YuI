using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_Reception_Ribbon.Room
{
    public enum ResMethod : byte 
    {
        ARRIVAL,
        BOOKING,
        HOSTELWORLD,
        AGODA,
        QUNAR,
        CTRIP,
        ELONG,
        QMANGO,
        MEITUAN,
        EMAIL,
        WECHAT,
        TELEPHONE,
        NHSC
    }

    public enum ResChannel : byte
    {
        PC,
        ANDROID,
        IPHONE,
        OTHER
    }

    public enum ResState
    {
        BOOKED,
        CHANGED,
        CANCELLED
    }

    public enum ResCheckState
    {
        NOSHOW,
        ADVANCE,
        DELAY,
        ARRIVED
    }

    public struct ResItem
    {
        public string ID { get; set; }
        public bool Checked { get; set; }
        public bool Replied { get; set; }
        public ResMethod Method { get; set; }
        public string MethodAccount { get; set; }
        public ResChannel Channel { get; set; }
        public Guest Guest { get; set; }
        public long TotalCost { get; set; }
        public long Deposit { get; set; }
        public long Balance { get; set; }
        public long Commission { get; set; }
        public ResState State { get; set; }
        public ResCheckState CheckState { get; set; }
        public DateTime BookedTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
        public byte Persons { get; set; }
        public byte Nights { get; set; }
        public byte Rooms { get; set; }
        public string ConfirmCode { get; set; }
        public DateTime Recorded { get; set; }
        public string Remarks { get; set; }
        public List<ResRoom> ResRooms { get; set; }
    }
}
