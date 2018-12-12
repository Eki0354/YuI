using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELite.ELiteItem
{

    #region GridRoomItem

    public struct GridRoomItem : IELiteTableItem
    {
        public long ID { get; set; }
        public int BUID { get; set; }
        public string ResNumber { get; set; }
        public long UID { get; set; }
        public string FullName { get; set; }
        public DateTime ReservedDate { get; set; }

        public override string ToString()
        {
            return FullName;
        }

        #region Static

        public static GridRoomItem Empty => new GridRoomItem()
        {
            ID = 42,
            BUID = 42,
            ResNumber = "42",
            UID = 42,
            FullName = "六娃",
            ReservedDate = new DateTime(2015, 6, 13)
        };

        #endregion

        #region IELiteItem

        public bool InsertTo(ELiteConnection conn)
        {
            return conn.Insert("log_bed", this.ToDictionary());
        }

        public bool DeleteFrom(ELiteConnection conn)
        {
            return conn.Delete("log_bed", "id", ID);
        }

        public bool UpdateIn(ELiteConnection conn)
        {
            return conn.Update("log_bed", this.ToDictionary(), "id", ID);
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "id", ID },
                { "BUID", BUID },
                { "ResNumber", ResNumber },
                { "uid", UID },
                { "ReservedDate", ReservedDate }
            };
        }

        public bool IsExistedIn(ELiteConnection conn)
        {
            return conn.IsExisted("log_bed", "id", ID);
        }

        public void LoadFromDataRow(DataRow row)
        {
            this.ID = (long)row["id"];
            this.BUID = (int)row["BUID"];
            this.ResNumber = (string)row["ResNumber"];
            this.UID = (long)row["uid"];
            this.FullName = (string)row["FullName"];
            this.ReservedDate = (DateTime)row["ReservedDate"];
        }

        #endregion

    }

    #endregion

    #region ELiteRoomItem

    public struct ELiteRoomItem:IELiteTableItem
    {
        public long ID { get; set; }
        public int RID { get; set; }
        public int RTID { get; set; }
        public int Floor { get; set; }
        public int Building { get; set; }
        public int Mode { get; set; }

        public override string ToString()
        {
            return RID.ToString();
        }

        #region Static

        public static ELiteRoomItem Empty => new ELiteRoomItem()
        {
            ID = 42,
            RID = 777,
            RTID = 42,
            Floor = 42,
            Building = 42,
            Mode = 42
        };

        #endregion

        #region IELiteItem

        public bool InsertTo(ELiteConnection conn)
        {
            return conn.Insert("info_room", this.ToDictionary());
        }

        public bool DeleteFrom(ELiteConnection conn)
        {
            return conn.Delete("info_room", "RID", RID);
        }

        public bool UpdateIn(ELiteConnection conn)
        {
            return conn.Update("info_room", this.ToDictionary(), "RID", RID);
        }

        public bool IsExistedIn(ELiteConnection conn)
        {
            return conn.IsExisted("info_room", "RID" + RID);
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "id", ID },
                { "RID", RID },
                { "RTID", RTID },
                { "Floor", Floor },
                { "Building", Building },
                { "Mode", Mode }
            };
        }

        public void LoadFromDataRow(DataRow row)
        {
            this.ID = (long)row["id"];
            this.RID = (int)row["RID"];
            this.RTID = (int)row["RTID"];
            this.Floor = (int)row["Floor"];
            this.Building = (int)row["Building"];
            this.Mode = (int)row["Mode"];
        }

        #endregion
        
    }

    #endregion

    #region ELiteRoomBuildingItem

    public struct ELiteRoomBuildingItem : IELiteTableItem
    {
        public int RBID { get; set; }
        public string Caption { get; set; }

        public override string ToString()
        {
            return Caption;
        }

        #region Static

        public static ELiteRoomBuildingItem Empty => new ELiteRoomBuildingItem()
        {
            RBID = 42,
            Caption = "巨山"
        };

        #endregion

        #region IELiteItem

        public bool InsertTo(ELiteConnection conn)
        {
            return conn.Insert("info_room_buildings", this.ToDictionary());
        }

        public bool DeleteFrom(ELiteConnection conn)
        {
            return conn.Delete("info_room_buildings", "RBID", RBID);
        }

        public bool UpdateIn(ELiteConnection conn)
        {
            return conn.Update("info_room_buildings", this.ToDictionary(), "RBID", RBID);
        }

        public bool IsExistedIn(ELiteConnection conn)
        {
            return conn.IsExisted("info_room_buildings", "RBID", RBID);
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "RBID", RBID },
                { "Caption", Caption }
            };
        }

        public void LoadFromDataRow(DataRow row)
        {
            this.RBID = (int)row["RBID"];
            this.Caption = (string)row["Caption"];
        }

        #endregion

    }

    #endregion

    #region ELiteRoomModeItem

    public struct ELiteRoomModeItem : IELiteTableItem
    {
        public int RMID { get; set; }
        public string Caption { get; set; }

        public override string ToString()
        {
            return Caption;
        }

        #region Static

        public static ELiteRoomModeItem Empty => new ELiteRoomModeItem()
        {
            RMID = 42,
            Caption = "Autobot"
        };

        #endregion

        #region IELiteItem

        public bool InsertTo(ELiteConnection conn)
        {
            return conn.Insert("info_room_modes", this.ToDictionary());
        }

        public bool DeleteFrom(ELiteConnection conn)
        {
            return conn.Delete("info_room_modes", "RMID", RMID);
        }

        public bool UpdateIn(ELiteConnection conn)
        {
            return conn.Update("info_room_modes", this.ToDictionary(), "RMID", RMID);
        }

        public bool IsExistedIn(ELiteConnection conn)
        {
            return conn.IsExisted("info_room_modes", "RMID", RMID);
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "RMID", RMID },
                { "Caption", Caption }
            };
        }

        public void LoadFromDataRow(DataRow row)
        {
            this.RMID = (int)row["RMID"];
            this.Caption = (string)row["Caption"];
        }

        #endregion

    }

    #endregion

    #region ELiteRoomOwnerItem

    public struct ELiteRoomOwnerItem : IELiteTableItem
    {
        public int ROID { get; set; }
        public string Caption { get; set; }

        public override string ToString()
        {
            return Caption;
        }

        #region Static

        public static ELiteRoomOwnerItem Empty => new ELiteRoomOwnerItem()
        {
            ROID = 42,
            Caption = "Landlord"
        };

        #endregion

        #region IELiteItem

        public bool InsertTo(ELiteConnection conn)
        {
            return conn.Insert("info_room_owners", this.ToDictionary());
        }

        public bool DeleteFrom(ELiteConnection conn)
        {
            return conn.Delete("info_room_owners", "ROID", ROID);
        }

        public bool UpdateIn(ELiteConnection conn)
        {
            return conn.Update("info_room_owners", this.ToDictionary(), "ROID", ROID);
        }

        public bool IsExistedIn(ELiteConnection conn)
        {
            return conn.IsExisted("info_room_owners", "ROID", ROID);
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "ROID", ROID },
                { "Caption", Caption }
            };
        }

        public void LoadFromDataRow(DataRow row)
        {
            this.ROID = (int)row["ROID"];
            this.Caption = (string)row["Caption"];
        }

        #endregion

    }

    #endregion

    #region ELiteRoomTypeItem

    public struct ELiteRoomTypeItem : IELiteTableItem
    {
        public int RTID { get; set; }
        public string Caption { get; set; }
        public int MaxOccupancy { get; set; }
        public string SingularEmailCaption_En { get; set; }
        public string PluralEmailCaption_En { get; set; }

        public override string ToString()
        {
            return Caption;
        }

        #region Static

        public static ELiteRoomTypeItem Empty => new ELiteRoomTypeItem()
        {
            RTID = 42,
            Caption = "Whistleblower",
            MaxOccupancy = 0,
            SingularEmailCaption_En = "Peanut Shell",
            PluralEmailCaption_En = "Peanut Shells"
        };

        #endregion

        #region IELiteItem

        public bool InsertTo(ELiteConnection conn)
        {
            return conn.Insert("info_room_types", this.ToDictionary());
        }

        public bool DeleteFrom(ELiteConnection conn)
        {
            return conn.Delete("info_room_types", "RTID", this.RTID);
        }

        public bool UpdateIn(ELiteConnection conn)
        {
            return conn.Update("info_room_types", this.ToDictionary(), "RTID", RTID);
        }

        public bool IsExistedIn(ELiteConnection conn)
        {
            return conn.IsExisted("info_room_types", "RTID", RTID);
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "RTID", RTID },
                { "Caption", Caption },
                { "MaxOccupancy", MaxOccupancy },
                { "SingularEmailCaption_En", SingularEmailCaption_En },
                { "PluralEmailCaption_En", PluralEmailCaption_En }
            };
        }

        public void LoadFromDataRow(DataRow row)
        {
            this.RTID = (int)row["RTID"];
            this.Caption = (string)row["Caption"];
            this.MaxOccupancy = (int)row["MaxOccupancy"];
            this.SingularEmailCaption_En = (string)row["SingularEmailCaption_En"];
            this.PluralEmailCaption_En = (string)row["PluralEmailCaption_En"];
        }

        #endregion

    }

    #endregion

    #region ELiteRoomTypeMatchItem

    public struct ELiteRoomTypeMatchItem : IELiteTableItem
    {
        public long ID { get; set; }
        public int RTID { get; set; }
        public string MatchChar { get; set; }

        public override string ToString()
        {
            return MatchChar;
        }

        #region Static

        public static ELiteRoomTypeMatchItem Empty => new ELiteRoomTypeMatchItem()
        {
            ID = 0,
            RTID = 42,
            MatchChar = "Nazo"
        };

        #endregion

        #region IELiteItem

        public bool InsertTo(ELiteConnection conn)
        {
            return conn.Insert("info_room_type_matches", this.ToDictionary());
        }

        public bool DeleteFrom(ELiteConnection conn)
        {
            return conn.Delete("info_room_type_matches", "id", ID);
        }

        public bool UpdateIn(ELiteConnection conn)
        {
            return conn.Update("info_room_type_matches", this.ToDictionary(), "id", ID);
        }

        public bool IsExistedIn(ELiteConnection conn)
        {
            return conn.IsExisted("info_room_type_matches", "id", ID);
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "id", ID },
                { "RTID", RTID },
                { "MatchChar", MatchChar }
            };
        }

        public void LoadFromDataRow(DataRow row)
        {
            this.ID = (long)row["id"];
            this.RTID = (int)row["RTID"];
            this.MatchChar = (string)row["MatchChar"];
        }

        #endregion

    }

    #endregion

}
