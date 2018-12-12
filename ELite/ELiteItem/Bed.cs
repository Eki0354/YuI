using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELite;
using System.Data;

namespace ELite.ELiteItem
{

    #region ELiteBedItem

    public struct ELiteBedItem : IELiteTableItem
    {
        public int BUID { get; set; }
        public int RID { get; set; }
        public int BID { get; set; }
        public int BTID { get; set; }
        public int State { get; set; }
        public string ResNumber { get; set; }
        
        public override string ToString()
        {
            return BUID.ToString();
        }

        #region Static

        public static ELiteBedItem Empty => new ELiteBedItem()
        {
            BUID = 42,
            RID = 777,
            BID = 42,
            BTID = 42,
            State = 42,
            ResNumber = "42"
        };
        
        #endregion

        #region IELiteItem

        public bool InsertTo(ELiteConnection conn)
        {
            return conn.Insert("info_bed", this.ToDictionary());
        }

        public bool DeleteFrom(ELiteConnection conn)
        {
            return conn.Delete("info_bed", "BUID", BUID);
        }

        public bool UpdateIn(ELiteConnection conn)
        {
            return conn.Update("info_bed", this.ToDictionary(), "BUID", BUID);
        }

        public bool IsExistedIn(ELiteConnection conn)
        {
            return conn.IsExisted("info_bed", "BUID", BUID);
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "BUID", BUID },
                { "RID", RID },
                { "BID", BID },
                { "BTID", BTID },
                { "State", State },
                { "ResNumber", ResNumber }
            };
        }

        public void LoadFromDataRow(DataRow row)
        {
            this.BUID = (int)row["BUID"];
            this.RID = (int)row["RID"];
            this.BID = (int)row["BID"];
            this.BTID = (int)row["BTID"];
            this.State = (int)row["State"];
            this.ResNumber = (string)row["ResNumber"];
        }

        #endregion

    }

    #endregion

    #region ELiteBedStateItem

    public struct ELiteBedStateItem : IELiteTableItem
    {
        public int BSID { get; set; }
        public string Caption { get; set; }

        public override string ToString()
        {
            return Caption;
        }

        #region Static

        public static ELiteBedStateItem Empty => new ELiteBedStateItem()
        {
            BSID = 42,
            Caption = "幽玄"
        };

        #endregion

        #region IELiteItem

        public bool InsertTo(ELiteConnection conn)
        {
            return conn.Insert("info_bed_states", this.ToDictionary());
        }

        public bool DeleteFrom(ELiteConnection conn)
        {
            return conn.Delete("info_bed_states", "BSID", BSID);
        }

        public bool UpdateIn(ELiteConnection conn)
        {
            return conn.Update("info_bed_states", this.ToDictionary(), "BSID", BSID);
        }

        public bool IsExistedIn(ELiteConnection conn)
        {
            return conn.IsExisted("info_bed_states", "BSID", BSID);
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "BSID", BSID },
                { "Caption", Caption }
            };
        }

        public void LoadFromDataRow(DataRow row)
        {
            this.BSID = (int)row["BSID"];
            this.Caption = (string)row["Caption"];
        }

        #endregion

    }

    #endregion
    
    #region ELiteBedTypeItem

    public struct ELiteBedTypeItem : IELiteTableItem
    {
        public int BTID { get; set; }
        public string Caption { get; set; }
        public int Occupancy { get; set; }
        
        public override string ToString()
        {
            return Caption;
        }

        #region Static

        public static ELiteBedTypeItem Empty => new ELiteBedTypeItem()
        {
            BTID = 42,
            Caption = "花生皮",
            Occupancy = 42
        };

        #endregion

        #region IELiteItem

        public bool InsertTo(ELiteConnection conn)
        {
            
            return conn.Insert("info_bed_types", this.ToDictionary());
        }

        public bool DeleteFrom(ELiteConnection conn)
        {
            return conn.Delete("info_bed_types", "BTID", BTID);
        }

        public bool UpdateIn(ELiteConnection conn)
        {
            return conn.Update("info_bed_types", this.ToDictionary(), "BTID", BTID);
        }

        public bool IsExistedIn(ELiteConnection conn)
        {
            return conn.IsExisted("info_bed_types", "BTID", BTID);
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "BTID", BTID  },
                { "Caption", Caption },
                { "Occupancy", Occupancy }
            };
        }

        public void LoadFromDataRow(DataRow row)
        {
            this.BTID = (int)row["BTID"];
            this.Caption = (string)row["Caption"];
            this.Occupancy = (int)row["Occupancy"];
        }

        #endregion

    }

    #endregion
    
}
