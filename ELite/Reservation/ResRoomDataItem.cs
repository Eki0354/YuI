using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELite.Reservation
{
    public struct ResRoomDataItem
    {
        public long ResID { get; set; }
        public DateTime ReservedDate { get; set; }
        public int Type { get; set; }
        public string FullName { get; set; }
        public int Rooms { get; set; }

        public override string ToString()
        {
            return FullName;
        }

        #region Static

        public static ResRoomDataItem Empty => new ResRoomDataItem()
        {
            ResID = 42,
            ReservedDate = new DateTime(2015, 6, 13),
            Type = 42,
            FullName = "六娃",
            Rooms = 0
        };

        public static ResRoomDataItem FromDataRow(DataRow row)
        {
            try
            {
                return new ResRoomDataItem()
                {
                    ResID = (long)row["ResID"],
                    ReservedDate = (DateTime)row["ReservedDate"],
                    Type = (int)row["Type"],
                    FullName = (string)row["FullName"],
                    Rooms = (int)row["Rooms"]
                };
            }
            catch
            {
                return ResRoomDataItem.Empty;
            }
        }

        #endregion

    }
}
