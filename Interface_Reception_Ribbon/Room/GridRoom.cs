using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface_Reception_Ribbon.Room
{
    public enum RoomState
    {
        OK,
        MAKINGUP,
        UNAVAILABLE
    }

    public struct GridRoom
    {
        public RoomItem Room { get; set; }
        public ResItem Res { get; set; }
        public RoomState State { get; set; }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
