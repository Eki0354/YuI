using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELite.ELiteItem;
using ELite.GridItem;
using ELite.Room;

namespace ELite
{
    public partial class ELiteConnection
    {
        public Dictionary<long, string> RoomBuildings { get; set; }
        public Dictionary<int, string> RoomTypes { get; set; }

        private void InitializeRoomProperty()
        {
            RoomBuildings = GetItemDictionary<long, string>("info_room_buildings", "id", "Caption", "", "id");
            RoomTypes = GetItemDictionary<int, string>("info_room_types", "RTID", "Caption", "", "RTID");
        }

        public List<ELiteRoomTypeItem> GetAllRoomType()
        {
            return FromSqlString<ELiteRoomTypeItem>("select * from info_room_types");
        }

        #region RoomTypeMatch

        public List<string> GetAllRoomTypeMatchChar()
        {
            return GetItems<string>("select distinct Type from info_res_rooms");
        }

        public List<ELiteRoomTypeMatchItem> GetMatchedRoomTypeMatchChar()
        {
            return FromSqlString<ELiteRoomTypeMatchItem>("select * from info_room_type_matches");
        }

        public List<string> GetUnmatchedTypeMatchChar()
        {
            return GetItems<string>("select distinct Type from info_res_rooms where Type not in " +
                "(select MatchChar from info_room_type_matches)");
        }

        #endregion

        #region GridRoomItem

        public DataTable GetEmptyGridRoomItemSet(DateTime date_start, DateTime date_end)
        {
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn()
            {
                ColumnName = "房间号",
                DataType = typeof(string)
            });
            for (DateTime date = date_start; date <= date_end; date = date.AddDays(1))
            {
                result.Columns.Add(new DataColumn()
                {
                    ColumnName = date.ToString("MM月dd日"),
                    DataType = typeof(GridRoomItem)
                });
            }
            GetGridRoomNumberList().ForEach(roomNumber =>
            {
                DataRow row = result.NewRow();
                row[0] = roomNumber;
                result.Rows.Add(row);
            });
            return result;
        }

        public DataTable GetGridRoomItemSet(DateTime date_start, DateTime date_end)
        {
            List<GridRoomItem> source = FromSqlString<GridRoomItem>
                ("select log_bed.id,BUID,ResNumber,log_bed.uid, info_user.FullName,ReservedDate " +
            "from log_bed,info_user where ReservedDate>='" + date_start.ToString("yyyy-MM-dd") + "' and ReservedDate<='" +
            date_end.ToString("yyyy-MM-dd") + "' and log_bed.uid=info_user.uid");
            DataTable result = GetEmptyGridRoomItemSet(date_start, date_end);
            if (source.Count < 1) return result;
            List<int> buids = this.GetItems<int>("info_bed", "BUID", "BUID <> 42");
            source.ForEach(item =>
            {
                if (buids.Contains(item.BUID) && item.ReservedDate >= date_start)
                    result.Rows[buids.IndexOf(item.BUID)][(item.ReservedDate - date_start).Days + 1] = item;
            });
            return result;
        }

        public List<string> GetGridRoomNumberList()
        {
            List<string> rnList = new List<string>();
            List<ELiteRoomItem> rooms = FromSqlString<ELiteRoomItem>("select * from info_room");
            FromSqlString<ELiteBedItem>("select * from info_bed where BUID<>42").ForEach(bed =>
            {
                switch (rooms.Find(room => room.RID == bed.RID).Mode)
                {
                    case 1:
                        rnList.Add(bed.RID.ToString());
                        break;
                    case 2:
                        rnList.Add(bed.RID + "-" + bed.BID);
                        break;
                    default:
                        rnList.Add(bed.ToString());
                        break;
                }
            });
            rnList = rnList.Distinct().ToList();
            return rnList;
        }

        #endregion

        public void AutoArrangeRes()
        {
            SQLiteTransaction tran = _Conn.BeginTransaction();
            DataTable resSet = Select("select info_res_rooms.Type,info_res_rooms.ResNumber,info_res_rooms.uid," +
                "info_res_rooms.ReservedDate,info_res.Deposit,info_res_rooms.Price,info_res_rooms.Persons,info_res_rooms.Rooms " +
                "from info_res,info_res_rooms where info_res.ResNumber=info_res_rooms.ResNumber");
            Dictionary<string, object> items = new Dictionary<string, object>()
            {
                { "BUID", 42 },
                { "ResNumber", "42" },
                { "uid", 0 },
                { "ReservedDate", new DateTime(2015,6,13) },
                { "Deposit", 0 },
                { "Price", 42 },
            };
            foreach(DataRow row in resSet.Rows)
            {
                long rooms = (long)row["Rooms"];
                if (rooms == 0)
                {
                    int maxOccupancy = (int)ReadValue("select MaxOccupancy from info_room_types where " +
                        "RTID=(select RTID from info_room_type_matches where MatchChar='" + (string)row["Type"] + "')");
                    rooms = (long)row["Persons"] / maxOccupancy;
                }
                items["BUID"] = ReadValue("select BUID from info_bed where RID in (" +
                    "select RID from info_room where RTID in (" +
                    "select RTID from info_room_type_matches where MatchChar='" + (string)row["Type"] +
                    "') limit 1) limit 1");
                items["ResNumber"] = (string)row["ResNumber"];
                items["uid"] = (long)row["uid"];
                items["ReservedDate"] = (DateTime)row["ReservedDate"];
                items["Deposit"] = (double)row["Deposit"];
                items["Price"] = (double)row["Price"];
                for(long i = 0; i < rooms; i++)
                {
                    Insert("log_bed", items);
                }
            }
            tran.Commit();
        }
    }

}
