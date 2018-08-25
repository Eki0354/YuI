using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELite.Reservation
{
    public struct ResChannel
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public string Title_zh_cn { get; set; }
        public string Title_en_us { get; set; }
        public string AbTitle { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public int RoomDateType { get; set; }
    }
}
