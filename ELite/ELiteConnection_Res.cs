using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml;
using ELite.Reservation;

namespace ELite
{
    public partial class ELiteConnection
    {
        public static Dictionary<int, string> ResStatus = new Dictionary<int, string>()
        {
            {0,"Ok" },
            {1,"已更改" },
            {2,"已取消" }
        };
        public static List<ResChannel> Channels;

        #region NEED

        private void InitializeChannels()
        {
            Channels = new List<ResChannel>();
            foreach (XmlNode node in _XmlReader.ReadNodes("Channels"))
            {
                string channelName = node.Name;
                Dictionary<string, string> arrs = _XmlReader.ReadPairs("Channels/" + channelName);
                Channels.Add(new ResChannel()
                {
                    Name = channelName,
                    ID = Convert.ToInt32(arrs["ID"]),
                    Title_zh_cn = arrs["Title_zh_cn"],
                    Title_en_us = arrs["Title_en_us"],
                    AbTitle = arrs["AbTitle"],
                    Account = arrs["Account"],
                    Password = arrs["Password"],
                    RoomDateType = Convert.ToInt32(arrs["RoomDateType"])
                });
            }
        }

        public bool ExistRes(string resNumber)
        {
            string sqlString = "select count(*) from info_res where ResNumber='" + resNumber + "'";
            _Comm.CommandText = sqlString;
            return (int)(_Comm.ExecuteScalar()) > 0;
        }

        public bool ExistResRooms(string resNumber)
        {
            string sqlString = "select count(*) from info_res_rooms where ResNumber='" + resNumber + "'";
            _Comm.CommandText = sqlString;
            return (int)(_Comm.ExecuteScalar()) > 0;
        }

        #endregion

        #region SHOW

        public string FilterResNumber(string condition)
        {
            if (condition == null || condition == "") return null;
            string sqlString = "select ResNumber from info_res where " + condition;
            _Comm.CommandText = sqlString;
            return (string)_Comm.ExecuteScalar();
        }

        public List<string> FilterResNumberList(string condition)
        {
            if (condition == null || condition == "") return null;
            string sqlString = "select ResNumber from info_res where " + condition;
            DataTable dt = Select(sqlString);
            if (dt.Rows.Count < 1) return null;
            List<string> resList = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                resList.Add(row[0].ToString());
            }
            return resList;
        }

        public DataTable SelectUncheckedRes()
        {
             return Select("select Channel, ResNumber from info_res where Checked=0");
        }
        
        public Booking GetBooking(ListBoxResItem res, out DataTable roomDT)
        {
            string sqlString = "select * from info_res,info_user where " +
                "info_res.uid=info_user.uid and info_res.ResNumber='" + res.ResNumber + "'";
            DataTable dt = Select(sqlString);
            roomDT = GetResRoomsDataTable(res);
            if (dt.Rows.Count < 1) return null;
            Dictionary<string, object> resDict = new Dictionary<string, object>();
            Dictionary<string, object> resUserDict = new Dictionary<string, object>();
            int resItemsCount = 21;
            for(int ci = 0; ci < resItemsCount; ci++)
            {
                resDict.Add(dt.Columns[ci].ColumnName, dt.Rows[0][ci]);
            }
            resUserDict.Add(dt.Columns[resItemsCount].ColumnName.Substring(0, 3),
                dt.Rows[0][resItemsCount]);
            for (int ci=resItemsCount+1; ci < dt.Columns.Count; ci++)
            {
                resUserDict.Add(dt.Columns[ci].ColumnName, dt.Rows[0][ci]);
            }
            List<Dictionary<string, object>> resRoomDictList = new List<Dictionary<string, object>>();
            for(int ri = 0; ri < roomDT.Rows.Count; ri++)
            {
                Dictionary<string, object> resRoomDict = new Dictionary<string, object>();
                for (int ci = 0; ci < roomDT.Columns.Count; ci++)
                {
                    resRoomDict.Add(roomDT.Columns[ci].ColumnName, roomDT.Rows[ri][ci]);
                }
                resRoomDictList.Add(resRoomDict);
            }
            return new Booking(resDict, resUserDict, resRoomDictList);
        }

        private DataTable GetResRoomsDataTable(ListBoxResItem res)
        {
            ResChannel resChannel = Channels.Find(c => c.Name == res.Channel);
            string dateKeys = resChannel.RoomDateType == 0 ?
                ("ArrivalDate, DepartureDate, ") : "ReservedDate, ";
            string sqlString = "select Type, " + dateKeys +
                "Price, Persons, Rooms, Nights, Status from info_res_rooms" +
                " where ResNumber='" + res.ResNumber + "'";
            DataTable dt = Select(sqlString);
            #region 重建新的DataTable，初始化日期和订单状态显示格式

            DataTable result = dt.Clone();
            result.Columns[1].DataType = typeof(string);
            if (resChannel.RoomDateType == 0)
                result.Columns[2].DataType = typeof(string);
            result.Columns[result.Columns.Count - 1].DataType = typeof(string);
            for (int ri = 0; ri < dt.Rows.Count; ri++)
            {
                DataRow row = result.NewRow();
                for (int ci = 0; ci < dt.Columns.Count; ci++)
                {
                    if (ci == 1 || (ci == 2 && resChannel.RoomDateType == 0))
                        row[ci] = (Convert.ToDateTime(dt.Rows[ri][ci]))
                            .ToString("yyyy年MM月dd日");
                    else if (ci == dt.Columns.Count - 1)
                        row[ci] = ResStatus[Convert.ToInt32(dt.Rows[ri][ci])];
                    else
                        row[ci] = dt.Rows[ri][ci];
                }
                result.Rows.Add(row);
            }

            #endregion
            return result;
        }

        #endregion

        public void InsertRes(Booking booking)
        {
            string sqlString = booking.ResItem.ToInsertString();
            Run("insert into info_res " + booking.ResItem.ToInsertString(),
                Operation.INSERTRES, "info_res");
            Run("insert into info_user " + booking.ResUserItem.ToInsertString(),
                Operation.INSERT, "info_user");
            booking.ResRoomItemList.ForEach(resRoomItem => Run("insert into info_res_rooms " +
                resRoomItem.ToInsertString(), Operation.INSERTRESROOM, "info_res_rooms"));
        }

        public void UpdateRes(string resNumber, string key, object value)
        {
            Run("update info_res set " + key + "='" + ItemToString(value) + "'");
        }

        #region DELETE

        public void DeleteResByCondition(string condition = "")
        {
            if (condition == null || condition == "") return;
            FilterResNumberList(condition).ForEach(res => DeleteResByNumber(res));
        }

        public void DeleteResByNumber(string resNumber)
        {
            Run("delete from info_res where ResNumber='" + resNumber + "'",
                Operation.DELETERES, "info_res");
            Run("delete from info_res_rooms where ResNumber='" + resNumber + "'",
                Operation.DELETERES, "info_res_rooms");
        }

        #endregion

    }
}
