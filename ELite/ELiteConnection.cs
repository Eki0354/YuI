using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.IO;
using ErrorLogger;
using EkiXmlDocument;
using Staff;

namespace ELite
{
    public enum Operation
    {
        RUN,
        INSERT,
        UPDATE,
        DELETE,
        INSERTRES,
        INSERTRESROOM,
        UPDATETABLE,
        UPDATERES,
        UPDATERESROOM,
        DELETETABLE,
        DELETERES,
        DELETERESROOM
    }

    public partial class ELiteConnection
    {
        private SQLiteConnection _Conn;
        private SQLiteCommand _Comm;
        private SQLiteDataReader _DataReader;
        private string _Path;
        private string _DBPath { get { return _Path + "\\database"; } }
        private string _Name;
        private string _Extension;
        public string FullPath { get { return _DBPath + "\\" + _Name + "." + _Extension; } }
        private string _BackUpPath { get { return _Path + "\\backup"; } }
        private string _Password;
        private Logger _Logger;
        private EXmlReader _XmlReader;
        private StaffItem _Staff;

        #region BASE

        public ELiteConnection(string fullPath = "", string password = "")
        {
            InitializePath(fullPath);
            _Password = password;
            Open();
            InitializeLogger();
            InitializeXmlReader();
        }

        public ELiteConnection(string path, string name = "min",
            string extension = "db", string password = "")
        {
            InitializePath(path, name, extension);
            _Password = password;
            Open();
            InitializeLogger();
            InitializeXmlReader();
        }

        public void Open()
        {
            if (!File.Exists(FullPath))
                File.Create(FullPath);
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder()
            {
                DataSource = FullPath,
                Password = _Password
            };
            _Conn = new SQLiteConnection() { ConnectionString = builder.ConnectionString };
            _Conn.Open();
            _Comm = new SQLiteCommand(_Conn);
        }

        public void Close()
        {
            if (_Logger != null)
                _Logger.Close();
            _Logger = null;

            if (_XmlReader != null)
                _XmlReader.Close();
            _XmlReader = null;

            if (_Comm != null)
                _Comm.Cancel();
            _Comm = null;

            if (_DataReader != null)
                _DataReader.Close();
            _DataReader = null;

            if (_Conn != null)
                _Conn.Close();
            _Conn = null;
        }

        public void BackUp()
        {
            File.Copy(_Path, _BackUpPath +
                "\\" + DateTime.Now.ToString("yyyyMMdd HHmmss.fff") + ".bk");
        }

        public void RefreshXmlReader()
        {
            _XmlReader.Refresh();
        }

        #endregion

        #region INITIALIZE

        private void InitializePath(string fullPath)
        {
            int index0 = fullPath.LastIndexOf("\\");
            int index1 = fullPath.LastIndexOf(".");
            if (fullPath != "" && index0 > 0 && index1 > 1 && index0 < index1)
            {
                _Path = fullPath.Substring(0, index0);
                _Name = fullPath.Substring(index0 + 1, index1 - index0 - 1);
                _Extension = fullPath.Substring(index1 + 1);
            }
            else
            {
                _Path = Environment.CurrentDirectory;
                _Name = "min";
                _Extension = "db";
            }
        }

        private void InitializePath(string path, string name, string extension)
        {
            if (path == "") path = Environment.CurrentDirectory;
            if (name == "") name = "min";
            if (extension == "") extension = "db";
            _Path = path;
            _Name = name;
            _Extension = extension;
        }

        private void InitializeLogger()
        {
            _Logger = new Logger(_DBPath, _Name + "." + _Extension);
            _Logger.Open();
        }

        private void InitializeXmlReader()
        {
            _XmlReader = new EXmlReader(_Path);
            _XmlReader.Open();
        }

        #endregion

        #region USER

        public void SetStaff(StaffItem staff)
        {
            if (_Staff != staff) _Staff = staff;
        }

        #endregion

        #region OPERATIONS_BASE

        private object ReadValue(string table, string key)
        {
            string sqlString;
            sqlString = "select " + key + " from " + table + " order by id desc";
            _Comm.CommandText = sqlString;
            return _Comm.ExecuteScalar();
        }

        private string ReadValue(string table, List<string> keys = null)
        {
            string sqlString;
            if (keys == null || keys.Count == 0)
                sqlString = "select * ";
            else
                sqlString = "select " + String.Join(",", keys.ToArray());
            sqlString += " from " + table;
            DataTable dt = Select(sqlString);
            if (dt.Rows.Count < 1) return "";
            DataRow row = dt.Rows[0];
            string result = "";
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                result += row[i].ToString();
            }
            return result;
        }

        private int MaxId(string table, string key = "id")
        {
            string sqlString = "select " + key + " from " + table + " order by " + key + " desc";
            _Comm.CommandText = sqlString;
            object result = _Comm.ExecuteScalar();
            if (result == null)
                return -1;
            else
                return (int)result;
        }

        private void AddOperationLog(string table, Operation operation)
        {
            Dictionary<string, object> items = new Dictionary<string, object>()
            {
                {"sid", _Staff.Items.SID },
                {"Table", table },
                {"Operation", (int)operation },
                {"Logged",DateTime.Now.ToString("yyyyMMdd HHmmss.ttt") }
            };
            string sqlString = "insert into log_operation " + ItemsToApartString(items);
            Run(sqlString, Operation.RUN);
        }

        private void Run(string sql, Operation operation = Operation.RUN, string table = "")
        {
            _Comm.CommandText = sql;
            try
            {
                _Comm.ExecuteNonQuery();
                if (table != "")
                    AddOperationLog(table, operation);
            }
            catch (Exception e)
            {
                _Logger.Log(sql, e.Message);
            }
        }

        private SQLiteDataReader Read(string sql)
        {
            if (sql == null || sql == "") return null;
            _Comm.CommandText = sql;
            try
            {
                _DataReader = _Comm.ExecuteReader();
            }
            catch (Exception e)
            {
                _Logger.Log(sql, e.Message);
                _DataReader = null;
            }
            return _DataReader;
        }

        private DataTable Select(string sql)
        {
            if (sql == null || sql == "") return null;
            string tableName = "反正是个没有人看得到的彩蛋，那我写得长一点也没关系吧";
            SQLiteDataAdapter da = new SQLiteDataAdapter(sql, _Conn);
            DataSet ds = new DataSet();
            da.Fill(ds, tableName);
            return ds.Tables[tableName];
        }

        #endregion

        #region TABLES

        private bool ExistTable(string name)
        {
            string sql = "select count(*) from sqlite_master where type='table' and name=" + name;
            _Comm.CommandText = sql;
            return (int)(_Comm.ExecuteScalar()) != 0;
        }

        private void DeleteTable(string tableName)
        {
            if (!ExistTable(tableName)) return;
            string sql = "drop table " + tableName;
            Run(sql, Operation.DELETETABLE, tableName);
        }

        #endregion

        #region SELECT & Read

        private SQLiteDataReader Read(string table, List<string> keys,
            string key = "", object value = null, string condition = "", string orderKey = "")
        {
            if (table == null || table == "") return null;
            string sqlString = GetSelectString(table, keys, key, value, condition, orderKey);
            return Read(sqlString);
        }

        private string GetSelectString(string table, List<string> keys, string key,
            object value, string condition, string orderKey)
        {
            string sqlString = "";
            if (keys == null || keys.Count == 0)
                sqlString = "select * from " + table;
            else
                sqlString = "select " + String.Join(",", keys.ToArray()) + " from " + table;
            string conditionString = "";
            if (condition != "")
                conditionString = condition;
            else if (key != "")
                conditionString = key + "='" + ItemToString(value) + "'";
            if (orderKey != "")
                conditionString += " order by " + orderKey;
            if (conditionString != "")
                sqlString += " where " + conditionString;
            return sqlString;
        }

        private DataTable Select(string table, List<string> keys,
            string key, object value, string condition, string orderKey = "")
        {
            if (table == null || table == "") return null;
            string sqlString = GetSelectString(table, keys, key, value, condition, orderKey);
            return Select(sqlString);
        }

        #endregion

        #region INSERT

        private void Insert(string table, Dictionary<string, object> items,
            Operation operation = Operation.INSERT)
        {
            if (table == null || table == "" || items == null || items.Count == 0) return;
            string sqlString = "insert into " + table + " " + ItemsToApartString(items);
            Run(sqlString, operation, table);
        }

        #endregion

        #region UPDATE

        private void Update(string table, Dictionary<string, object> items,
            Operation operation = Operation.UPDATE, string key = "", object value = null,
            string condition = "")
        {
            if (table == null || table == "" || items == null || items.Count == 0) return;
            string sqlString = GetUpdateString(table, items, key, value, condition);
            Run(sqlString, operation, table);
        }
        
        private string GetUpdateString(string table, Dictionary<string, object> items,
            string key, object value, string condition)
        {
            string sqlString = "update " + table + " set " + ItemsToCombineString(items);
            if (key != "")
                sqlString += " where " + key + "='" + ItemToString(value) + "'";
            else if (condition != "")
                sqlString += " where " + condition;
            return sqlString;
        }

        #endregion

        #region DELETE

        private void Delete(string table, Operation operation = Operation.DELETE,
            string key = "", object value = null, string condition = "")
        {
            if (table == null || table == "") return;
            string sqlString = GetDeleteString(table, key, value, condition);
            Run(sqlString, operation, table);
        }

        private string GetDeleteString(string table, string key, object value, string condition)
        {
            string sqlString = "";
            if (key != "")
                sqlString = "delete from " + table + " where " +
                    key + "='" + ItemToString(value) + "'";
            else if (condition != "")
                sqlString = "delete from " + table + " where " + condition;
            else
                sqlString = "delete from " + table;
            return sqlString;
        }

        #endregion

        #region SHARED

        private string ItemsToCombineString(Dictionary<string, object> items)
        {
            string result = "";
            foreach (KeyValuePair<string, object> item in items)
            {
                result += "," + item.Key + "='" + ItemToString(item.Value) + "'";
            }
            if (result == "")
                return result;
            else
                return result.Substring(1);
        }

        private string ItemsToApartString(Dictionary<string, object> items)
        {
            return "(" + KeysToString(items) + ") values(" + ValuesToString(items);
        }

        private string KeysToString(Dictionary<string, object> items)
        {
            return String.Join(",", items.Keys.ToArray());
        }

        private string ValuesToString(Dictionary<string, object> items)
        {
            string valueString = "";
            foreach (object obj in items.Values)
            {
                valueString += ",'" + ItemToString(obj) + "'";
            }
            if (valueString == "")
                return valueString;
            else
                return valueString.Substring(1);
        }

        private string ItemToString(object obj)
        {
            string objString = "";
            if (obj != null)
            {
                Type type = obj.GetType();
                if (type == typeof(DateTime))
                {
                    objString = ((DateTime)obj).ToString("yyyy/MM/dd HH:mm:ss");
                }
                else if (type == typeof(bool))
                {
                    objString = ((bool)obj) ? "1" : "1";
                }
                else
                {
                    objString = obj.ToString();
                }
            }
            return objString;
        }

        #endregion

    }
}
