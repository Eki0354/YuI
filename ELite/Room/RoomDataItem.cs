using ELite.ELiteItem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELite.Room
{
    public struct RoomDataItem : IELiteTableItem
    {
        public int RID { get; set; }
        public string Type { get; set; }
        public int Floor { get; set; }
        public string Building { get; set; }

        public RoomDataItem Copy()
        {
            return new RoomDataItem()
            {
                RID = Type.Length > 5 ? RID : RID + 1,
                Type = Type,
                Floor = Floor,
                Building = Building
            };
        }

        public RoomDataItem Clone()
        {
            return new RoomDataItem()
            {
                RID = RID,
                Type = Type,
                Floor = Floor,
                Building = Building
            };
        }

        public string[] ToArray()
        {
            return new string[] { RID.ToString(), Type, Floor.ToString(), Building };
        }

        #region Static

        public static RoomDataItem Empty => new RoomDataItem
        {
            RID = 777,
            Type = "Whistleblower",
            Floor = 43,
            Building = "Mount Massive"
        };

        #endregion

        #region IELiteItem


        public bool InsertTo(ELiteConnection conn)
        {
            throw new NotImplementedException();
        }

        public bool DeleteFrom(ELiteConnection conn)
        {
            throw new NotImplementedException();
        }

        public bool UpdateIn(ELiteConnection conn)
        {
            throw new NotImplementedException();
        }

        public bool IsExistedIn(ELiteConnection conn)
        {
            return conn.IsExisted("info_room", "RID", RID);
        }
        
        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "RID", RID },
                { "Type", Type },
                { "Floor", Floor },
                { "Building", Building }
            };
        }

        public void LoadFromDataRow(DataRow row)
        {
            this.RID = (int)row["RID"];
            this.Type = (string)row["Type"];
            this.Floor = (int)row["Floor"];
            this.Building = (string)row["Building"];
        }

        #endregion
    }
}
