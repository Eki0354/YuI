using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_Reception_Ribbon.RoomPage
{
    public enum ResStatus { OK, CHANGED, CANCELLED }
    public enum PaymentStatus { UNPAID, DEPOSIT, PAID }
    public enum RoomStatus { OK, CHECKED_IN, CHECKED_OUT }

    public struct CellRoomItem
    {
        public int ID { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
