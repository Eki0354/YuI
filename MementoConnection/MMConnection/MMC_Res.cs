using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace MementoConnection
{
    public static partial class MMConnection
    {

        public static Dictionary<int, string> ResStatus = new Dictionary<int, string>()
        {
            {0,"Ok" },
            {1,"已更改" },
            {2,"已取消" }
        };

        #region NEED

        public static bool IsUserExisted(int uid) =>
            IsExisted("info_user", string.Format("uid='{0}'", uid));

        public static bool IsUserExisted(string fullName)
        {
            return GetUIDByFullName(fullName) > 0;
        }

        public static bool IsResRoomsExisted(string resNumber)
        {
            SQLiteCommand comm = new SQLiteCommand(
                "select count(*) from info_res_rooms where ResNumber='" + resNumber + "'", Conn);
            return (int)(comm.ExecuteScalar()) > 0;
        }

        public static long? GetUIDByFullName(string fullName) =>
            MaxValue<long>("info_user", "uid", "FullName='" +
                fullName.Replace("'", "''") + "' limit 1");

        public static long? GetUIDByEmail(string email) =>
            MaxValue<long>("info_user", "uid", "Email='" + email + "'");

        #endregion

        #region SHOW

        public static string FilterResNumber(string condition)
        {
            if (condition == null || condition == "") return null;
            return GetString("select ResNumber from info_res where " + condition);
        }

        public static List<string> FilterResNumberList(string condition) =>
            GetItems<string>("select ResNumber from info_res where " + condition);

        public static DataTable SelectUncheckedRes(this SQLiteConnection conn)
        {
            return Select("select Channel, ResNumber from info_res where IsChecked=0");
        }

        #endregion

        #region SELECT

        public static bool IsResExisted(string resNumber) =>
            IsExisted("info_res", string.Format("ResNumber='{0}'", resNumber));

        public static DataTable UnCheckedBubbleResSet=> Select(
            "select [id],State,Channel,ResNumber,FullName from info_res,info_user where " +
                "IsChecked=0 and info_res.uid=info_user.uid order by info_res.id desc limit 100");

        public static DataTable FindResByResNumberOrFullName(string keyword)
        {
            return Select(string.Format("select [id],State,Channel,ResNumber,FullName from info_res,info_user where " +
                "info_res.uid=info_user.uid and (ResNumber LIKE \'%{0}%\' OR " +
                "FullName LIKE \'%{0}%\')", keyword));
        }
        
        public static int? GetTypeByMatch(string matchText)
        {
            return GetItem<int>("info_room_typematches", "Type", 
                string.Format("MatchChar='{0}'", matchText));
        }

        #endregion

        #region INSERT

        public static bool InsertRes(
            Dictionary<string, object> items) => Insert("info_res", items);

        public static bool InsertResRoom(Dictionary<string, object> items)
        {
            if (!items.ContainsKey("uid") && items.ContainsKey("FullName"))
            {
                items.Add("uid", GetUIDByFullName(items["FullName"] as string).ToString());
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
                DateTime arrivalDate = (DateTime)items["ArrivalDate"];
                DateTime departureDate = (DateTime)(items["DepartureDate"]);
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

        public static long InsertUser(Dictionary<string, object> items)
        {
            string name = (items["FullName"] as string).Replace("茅", "é");
            if (name.Contains("&#"))
            {
                int i0 = name.IndexOf("&#");
                int i1 = name.IndexOf(";");
                items["FullName"] = name.Substring(0, i0) +
                    ((char)Convert.ToInt32(name.Substring(i0 + 2, i1 - i0 - 2))) +
                    name.Substring(i1 + 1);
            }
            long? uid = null;
            if (items.ContainsKey("Email"))
                GetUIDByEmail(items["Email"] as string);
            if (uid is null)
                uid = GetUIDByFullName(items["FullName"] as string);
            if (uid != null && uid > -1) return (int)uid;
            if (!Insert("info_user", items)) return -1;
            long? value = MaxValue<long>("info_user", "uid");
            return value is null ? -1 : (long)value;
        }

        public static bool InsertRes(string keys, string values)
        {
            return Execute("insert into info_res(" + keys + ") values(" + values + ")", Operation.INSERT, "info_res");
        }

        public static bool InsertResRoom(string keys, string values)
        {
            return Execute("insert into info_res_rooms(" + keys + ") values(" + values + ")", Operation.INSERT, "info_res_room");
        }

        #endregion

        #region UPDATE
        
        public static void UpdateResAllChecked(
            bool isChecked = true)=>
            ExecuteUpdate("info_res",
                new Dictionary<string, object>()
                {
                    { "IsChecked",isChecked }
                });

        public static void UpdateResChecked(
            string resNumber, bool isChecked = true) =>
            ExecuteUpdate("info_res",
                new Dictionary<string, object>()
                {
                    { "IsChecked",isChecked }
                },
                "ResNumber='" + resNumber + "'");

        public static void UpdateResState(string resNumber, int state = 0)
        {
            ExecuteUpdate("info_res",
                new Dictionary<string, object>()
                {
                    { "State",state }
                },
                "ResNumber='" + resNumber + "'");
        }

        #endregion

        #region DELETE

        public static void DeleteResByCondition(string condition = "") =>
            FilterResNumberList(condition).ForEach(res => DeleteResByResNumber(res));

        public static bool DeleteResByResNumber(string resNumber) =>
            Delete("info_res", "ResNumber='" + resNumber + "'") &&
            Delete("info_res_rooms", "ResNumber='" + resNumber + "'");

        #endregion

    }
}
