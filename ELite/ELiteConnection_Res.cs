using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ControlItemCollection;

namespace ELite
{
    public partial class ELiteConnection
    {

        #region SHARED

        public string GetResNumber(string condition)
        {
            if (condition == null || condition == "") return null;
            string sqlString = "select ResNumber from info_res where " + condition;
            _Comm.CommandText = sqlString;
            return (string)_Comm.ExecuteScalar();
        }

        public List<string> GetResNumberList(string condition)
        {
            if (condition == null || condition == "") return null;
            string sqlString = "select ResNumber from info_res where " + condition;
            DataTable dt = Select(sqlString);
            if (dt.Rows.Count < 1) return null;
            List<string> resList = new List<string>();
            foreach(DataRow row in dt.Rows)
            {
                resList.Add(row[0].ToString());
            }
            return resList;
        }

        #endregion

        #region PUBLIC

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

        public List<ListBoxResItem> UncheckedResList()
        {
            DataTable dt = Select("info_res", new List<string>() { "Channel", "ResNumber" },
                "Checked", "0", "", "id");
            List<ListBoxResItem> resList = new List<ListBoxResItem>();
            if (dt.Rows.Count < 1) return resList;
            foreach(DataRow row in dt.Rows)
            {
                resList.Add(new ListBoxResItem(row[0].ToString(), row[1].ToString()));
            }
            return resList;
        }

        public DataTable GetResRooms(string resNumber)
        {
            DataTable dt = ResRoomsDataTableSource(resNumber,new List<string>()
            {
                "uid",
                "Type",
                "ArrivalDate",
                "DepartureDate",
                "Price",
                "Persons",
                "Rooms",
                "Nights",
                "Status"
            });
            DataTable result = new DataTable();
            //添加列标题
            foreach(DataColumn column in dt.Columns)
            {
                result.Columns.Add(column.ColumnName);
            }
            //添加行数据，根据相应格式转换
            foreach (DataRow row in dt.Rows)
            {
                DataRow newRow = result.NewRow();
                //由uid获取客人姓名
                newRow[0] = ReadValue("info_user", 
                    new List<string>() { "Surname", "Givenname" });
                for(int i = 1; i < 9; i++)
                {
                    string value = "";
                    object obj = row[i];
                    if (obj.GetType() == typeof(DateTime))
                        value = ((DateTime)obj).ToString("yyyy年MM月dd日");
                    else
                        value = obj.ToString();
                    newRow[i] = value;
                }
                result.Rows.Add(newRow);
            }
            return result;
        }

        private DataTable ResRoomsDataTableSource(string resNumber, List<string> keys = null)
        {
            return Select("info_res_rooms", keys, "ResNumber", resNumber, "");
        }

        public void InsertRes(Dictionary<string, object> resItems,
            List<Dictionary<string, object>> roomItemsList)
        {
            Insert("info_res", resItems, Operation.INSERTRES);
            roomItemsList.ForEach(roomItems => 
                Insert("info_res_roomss", roomItems, Operation.INSERTRESROOM));
        }

        public void UpdateRes(Dictionary<string, object> resItems,
            List<Dictionary<string, object>> roomItemsList)
        {

        }

        public void DeleteResByCondition(string condition = "")
        {
            if (condition == null || condition == "") return;
            GetResNumberList(condition).ForEach(res => DeleteResByNumber(res));
        }

        public void DeleteResByNumber(string resNumber)
        {
            Delete("info_res", Operation.DELETERES, "ResNumber", resNumber);
            Delete("info_res_rooms", Operation.DELETERESROOM, "ResNumber", resNumber);
        }

        #endregion

    }
}
