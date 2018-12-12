using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml;
using ELite.Reservation;
using System.Data.SQLite;

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

        public bool IsUserExisted(int uid)
        {
            _Comm.CommandText = "select count(*) from info_user where uid=" + uid;
            return ((int)_Comm.ExecuteScalar()) > 0;
        }

        public bool IsUserExisted(string fullName)
        {
            return GetUIDByFullName(fullName) > 0;
        }

        public bool IsResExisted(string resNumber)
        {
            string sqlString = "select count(*) from info_res where ResNumber='" + resNumber + "'";
            _Comm.CommandText = sqlString;
            return ((int)_Comm.ExecuteScalar()) > 0;
        }

        public bool IsResRoomsExisted(string resNumber)
        {
            string sqlString = "select count(*) from info_res_rooms where ResNumber='" + resNumber + "'";
            _Comm.CommandText = sqlString;
            return (int)(_Comm.ExecuteScalar()) > 0;
        }

        public int GetUIDByFullName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return 42;
            _Comm.CommandText = "select uid from info_user where FullName like '" +
                fullName.Replace("'","''") + "'";
            object result = _Comm.ExecuteScalar();
            return result is null ? 0 : Convert.ToInt32(result);
        }

        #endregion

        #region SHOW

        public string FilterResNumber(string condition)
        {
            if (condition == null || condition == "") return null;
            return GetItem<string>("select ResNumber from info_res where " + condition);
        }

        public List<string> FilterResNumberList(string condition)
        {
            if (condition == null || condition == "") return null;
            return GetItems<string>("select ResNumber from info_res where " + condition);
        }

        public DataTable SelectUncheckedRes()
        {
            return Select("select Channel, ResNumber from info_res where IsChecked=0");
        }

        public Booking GetBooking(ELiteListBoxResItem res, out DataTable roomDT)
        {
            string sqlString = "select * from info_res,info_user where " +
                "info_res.uid=info_user.uid and info_res.ResNumber='" + res.ResNumber + "'";
            DataTable dt = Select(sqlString);
            roomDT = GetResRoomsDataTable(res);
            if (dt.Rows.Count < 1) return null;
            Dictionary<string, object> resDict = new Dictionary<string, object>();
            Dictionary<string, object> resUserDict = new Dictionary<string, object>();
            int resItemsCount = 21;
            for (int ci = 0; ci < resItemsCount; ci++)
            {
                resDict.Add(dt.Columns[ci].ColumnName, dt.Rows[0][ci]);
            }
            resUserDict.Add(dt.Columns[resItemsCount].ColumnName.Substring(0, 3),
                dt.Rows[0][resItemsCount]);
            for (int ci = resItemsCount + 1; ci < dt.Columns.Count; ci++)
            {
                resUserDict.Add(dt.Columns[ci].ColumnName, dt.Rows[0][ci]);
            }
            List<Dictionary<string, object>> resRoomDictList = new List<Dictionary<string, object>>();
            for (int ri = 0; ri < roomDT.Rows.Count; ri++)
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

        private DataTable GetResRoomsDataTable(ELiteListBoxResItem res)
        {
            ResChannel resChannel = Channels.Find(c => c.Name == res.Channel);
            string dateKeys = resChannel.RoomDateType == 0 ?
                ("ArrivalDate, DepartureDate, ") : "ReservedDate, ";
            string sqlString = "select Type, " + dateKeys +
                "Price, Persons, Rooms, Status from info_res_rooms" +
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

        #region SELECT

        #region Res

        public bool ExistValidRes(string resNumber)
        {
            _Comm.CommandText = "select * from info_res where ResNumber='" + resNumber + 
                "' and IsValid=1";
            return _Comm.ExecuteScalar() != null;
        }

        public List<ELiteListBoxResItem> UnCheckedListBoxResItems()
        {
            return FromSqlString<ELiteListBoxResItem>(
                "select [id],IsValid,Channel,ResNumber,FullName from info_res,info_user where " +
                "IsValid=1 and IsChecked=0 and info_res.uid=info_user.uid order by info_res.id " +
                "desc limit 100");
        }

        public List<ELiteListBoxResItem> FindResByResNumberOrFullName(string keyword)
        {
            return FromSqlString<ELiteListBoxResItem>(
                "select [id],IsValid,Channel,ResNumber,FullName from info_res,info_user where " +
                "info_res.uid=info_user.uid and (ResNumber LIKE \'%" + keyword + "%\' OR " +
                "FullName LIKE \'%" + keyword + "%\')");
        }

        #endregion

        #region ResRoom
        
        public ResRoomDataItem GetResRoomDataItemByID(int id)
        {
            DataTable items = Select("select * from info_res_rooms where id=" + id);
            if (id != 42 && items.Rows.Count < 1) return ResRoomDataItem.Empty;
            return ResRoomDataItem.FromDataRow(items.Rows[0]);
        }

        #endregion

        #endregion

        #region INSERT

        public void InsertRes(Booking booking)
        {
            string sqlString = booking.ResItem.ToInsertString();
            Run("insert into info_res " + booking.ResItem.ToInsertString(),
                Operation.INSERT, "info_res");
            Run("insert into info_user " + booking.ResUserItem.ToInsertString(),
                Operation.INSERT, "info_user");
            booking.ResRoomItemList.ForEach(resRoomItem => Run("insert into info_res_rooms " +
                resRoomItem.ToInsertString(), Operation.INSERT, "info_res_rooms"));
        }

        public bool InsertRes(Dictionary<string, string> items)
        {
            return Insert("info_res", items);
        }

        public bool InsertResRoom(Dictionary<string, string> items)
        {
            if (!items.ContainsKey("uid") && items.ContainsKey("FullName"))
            {
                items.Add("uid", GetUIDByFullName(items["FullName"]).ToString());
                items.Remove("FullName");
            }
            if (items.ContainsKey("ReservedDate"))
            {
                if (!Insert("info_res_rooms", items)) return false;
            }
            else
            {
                if (!items.ContainsKey("ArrivalDate") || !items.ContainsKey("DepartureDate"))
                    return false;
                DateTime arrivalDate = DateTime.Parse(items["ArrivalDate"]);
                DateTime departureDate = DateTime.Parse(items["DepartureDate"]);
                items.Remove("ArrivalDate");
                items.Remove("DepartureDate");
                items.Add("ReservedDate", string.Empty);
                for (DateTime dt = arrivalDate; dt < departureDate; dt = dt.AddDays(1))
                {
                    items["ReservedDate"] = dt.ToString("s");
                    if (!Insert("info_res_rooms", items)) return false;
                }
            }
            return true;
        }

        public int InsertUser(Dictionary<string, string> items)
        {
            string name = items["FullName"].Replace("茅", "é");
            if (name.Contains("&#"))
            {
                int i0 = name.IndexOf("&#");
                int i1 = name.IndexOf(";");
                items["FullName"] = name.Substring(0, i0) +
                    ((char)Convert.ToInt32(name.Substring(i0 + 2, i1 - i0 - 2))) +
                    name.Substring(i1 + 1);
            }
            int uid = GetUIDByFullName(items["FullName"]);
            if (uid > 0) return uid;
            if (!Insert("info_user", items)) return 0;
            return MaxValue("info_user", "uid");
        }

        public bool InsertRes(string keys, string values)
        {
            return Run("insert into info_res(" + keys + ") values(" + values + ")", Operation.INSERT, "info_res");
        }

        public bool InsertResRoom(string keys, string values)
        {
            return Run("insert into info_res_rooms(" + keys + ") values(" + values + ")", Operation.INSERT, "info_res_room");
        }

        #endregion

        #region UPDATE

        public void UpdateRes(string key, object value)
        {
            Run("update info_res set " + key + "='" + ToInputString(value) + "'");
        }

        public void UpdateRes(string resNumber, string key, object value, string conditon = "")
        {
            string sqlString = "update info_res set " + key + "='" + ToInputString(value) +
                "' where ResNumber='" + resNumber + "'";
            if (!String.IsNullOrEmpty(conditon))
                sqlString += " and " + conditon;
            Run(sqlString);
        }

        public void UpdateResAllChecked(bool isChecked = true)
        {
            UpdateRes("IsChecked", isChecked);
        }

        public void UpdateResChecked(string resNumber, bool isChecked = true)
        {
            UpdateRes(resNumber, "IsChecked", isChecked);
        }

        #endregion

        #region DELETE

        public void InvalidResByCondition(string condition = "")
        {
            if (condition == null || condition == "") return;
            FilterResNumberList(condition).ForEach(res => InvalidResByNumber(res));
        }

        public void InvalidResByNumber(string resNumber)
        {
            Run("update info_res set IsValid=0 where ResNumber='" + resNumber + "'",
                Operation.UPDATE, "info_res");
            //Run("delete from info_res_rooms where ResNumber='" + resNumber + "'",
                //Operation.DELETE, "info_res_rooms");
        }

        #endregion

    }
}
