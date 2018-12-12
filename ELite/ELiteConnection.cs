using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.IO;
using ELite;
using ELite.ELiteItem;

namespace ELite
{
    public enum Operation
    {
        RUN,
        INSERT,
        UPDATE,
        DELETE,
        CREATE
    }

    public partial class ELiteConnection
    {
        #region PROPERTY

        private SQLiteConnection _Conn;
        private SQLiteCommand _Comm;
        private SQLiteDataReader _DataReader;
        private string _Path;
        private string _Name;
        private string _Extension;
        public string FullName => _Name + "." + _Extension;
        public string FullPath => _Path + "\\" + FullName;
        private string _BackUpPath => _Path + "\\backup";
        private string _Password;
        private Logger _Logger;
        private EXmlReader _XmlReader;
        public EXmlReader XmlReader => _XmlReader;
        private StaffItem _Staff;

        #endregion
        
        #region BASE

        public ELiteConnection(string fullPath = "", string password = "")
        {
            InitializePath(fullPath);
            _Password = password;
            InitializeLogger();
            InitializeXmlReader();
            InitializeChannels();
        }

        public ELiteConnection(string path, string name = "min",
            string extension = "db", string password = "")
        {
            InitializePath(path, name, extension);
            _Password = password;
            InitializeLogger();
            InitializeXmlReader();
            InitializeChannels();
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
            InitializeRoomProperty();
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

        public SQLiteTransaction BeginTransaction()
        {
            return _Conn.BeginTransaction();
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
                _Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HROS";
                _Name = "min";
                _Extension = "db";
            }
        }

        private void InitializePath(string path, string name, string extension)
        {
            if (path == "")
                path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HROS\\database";
            else
                if (!path.EndsWith("database")) path += "\\database";
            if (name == "") name = "min";
            if (extension == "") extension = "db";
            _Path = path;
            _Name = name;
            _Extension = extension;
        }

        private void InitializeLogger()
        {
            _Logger = new Logger(_Path, FullName);
            _Logger.Open();
        }

        private void InitializeXmlReader()
        {
            _XmlReader = new EXmlReader(_Path, FullName);
            _XmlReader.Open();
        }

        #endregion

        #region STAFF

        public void SetStaff(StaffItem staff)
        {
            if (_Staff != staff) _Staff = staff;
        }

        #endregion

        #region Select

        public T FromDataRow<T>(DataRow row) where T : IELiteTableItem, new()
        {
            T t = new T();
            t.LoadFromDataRow(row);
            return t;
        }

        public List<T> FromDataTable<T>(DataTable table) where T : IELiteTableItem, new()
        {
            List<T> result = new List<T>();
            foreach(DataRow row in table.Rows)
            {
                result.Add(FromDataRow<T>(row));
            }
            return result;
        }

        public List<T> FromSqlString<T>(string sqlString) where T : IELiteTableItem, new()
        {
            DataTable table = Select(sqlString);
            return FromDataTable<T>(table);
        }

        public object ReadValue(string sqlString)
        {
            _Comm.CommandText = sqlString;
            return _Comm.ExecuteScalar();
        }

        public object ReadValue(string table, string key)
        {
            string sqlString;
            sqlString = "select " + key + " from " + table + " order by id desc";
            _Comm.CommandText = sqlString;
            return _Comm.ExecuteScalar();
        }

        /// <summary> 返回基本类型列表 </summary>
        public  List<T> GetItems<T>(string table, string keyName, string condition = "", string orderKey = "")
        {
            string sqlString = "select " + keyName + " from " + table;
            if (!String.IsNullOrEmpty(condition))
                sqlString += " where " + condition;
            if (!String.IsNullOrEmpty(orderKey))
                sqlString += " order by " + orderKey;
            return GetItems<T>(sqlString);
        }

        /// <summary> 返回基本类型列表 </summary>
        public List<T> GetItems<T>(string sqlString)
        {
            DataTable dt = Select(sqlString);
            List<T> items = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                items.Add((T)(row[0]));
            }
            return items;
        }

        /// <summary> 返回基本类型 </summary>
        public T GetItem<T>(string table, string keyName, string condition = "", string orderKey = "")
        {
            string sqlString = "select " + keyName + " from " + table;
            if (!String.IsNullOrEmpty(condition))
                sqlString += " where " + condition;
            if (!String.IsNullOrEmpty(orderKey))
                sqlString += " order by " + orderKey;
            return GetItem<T>(sqlString);
        }

        /// <summary> 返回基本类型 </summary>
        public T GetItem<T>(string sqlString)
        {
            DataTable dt = Select(sqlString);
            if (dt.Rows.Count < 1) return default(T);
            return (T)dt.Rows[0][0];
        }

        private Dictionary<TKey, TValue> GetItemDictionary<TKey, TValue>(string table, 
            string keyName, string valueName, string condition = "", string orderKey = "")
        {
            string sqlString = "select " + keyName + "," + valueName + " from " + table;
            if (!String.IsNullOrEmpty(condition))
                sqlString += " where " + condition;
            if (!String.IsNullOrEmpty(orderKey))
                sqlString += " order by " + orderKey;
            return GetItemDictionary<TKey, TValue>(sqlString);
        }

        private Dictionary<TKey, TValue> GetItemDictionary<TKey,TValue>(string sqlString)
        {
            DataTable dt = Select(sqlString);
            Dictionary<TKey, TValue> itemDict = new Dictionary<TKey, TValue>();
            foreach (DataRow row in dt.Rows)
            {
                itemDict.Add((TKey)row[0], (TValue)row[1]);
            }
            return itemDict;
        }

        public SQLiteDataReader Read(string sql, bool isDesc = false, int limitCount = 0)
        {
            if (sql == null || sql == "") return null;
            if (isDesc) sql += " desc";
            if (limitCount > 0) sql += " limit " + limitCount;
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

        public DataTable Select(string sql, bool isDesc = false, int limitCount = 0)
        {
            if (sql == null || sql == "") return null;
            if (isDesc) sql += " desc";
            if (limitCount > 0) sql += " limit " + limitCount;
            string tableName = "反正是个没有人看得到的彩蛋，那我写得长一点也没关系吧";
            SQLiteDataAdapter da = new SQLiteDataAdapter(sql, _Conn);
            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds, tableName);
            }
            catch(Exception ex)
            {
                Console.WriteLine(sql);
                throw ex;
            }
            return ds.Tables[tableName];
        }

        #endregion

        #region Insert
        
        internal bool Insert(string table, Dictionary<string, object> items)
        {
            return Run("insert into " + table + ToInsertString(items), Operation.INSERT, table);
        }

        internal bool Insert(string table, Dictionary<string, string> items)
        {
            return Run("insert into " + table + ToInsertString(items), Operation.INSERT, table);
        }

        #endregion

        #region Update

        internal bool Update(string table, Dictionary<string, object> items, string key, object value = null)
        {
            if (value == null && !items.ContainsKey(key)) return false;
            return Run("update " + table + " set " + ToUpdateString(items) +
                " where " + key + "='" + ToInputString(value) + "'", Operation.UPDATE, table);
        }

        #endregion

        #region Delete

        internal bool Delete(string table, string key, object value)
        {
            if (string.IsNullOrEmpty(key)) return false;
            string valueString = ToInputString(value);
            if (string.IsNullOrEmpty(valueString)) return false;
            return Run("delete from " + table + " where " + key + "='" + valueString + "'", Operation.DELETE, table);
        }

        #endregion

        #region OPERATIONS_BASE

        internal bool IsExisted(string sqlString)
        {
            _Comm.CommandText = sqlString;
            return _Comm.ExecuteScalar() != null;
        }

        internal bool IsExisted(string table, string condition)
        {
            return IsExisted("select * from " + table + " where " + condition);
        }

        internal bool IsExisted(string table, string key, object value)
        {
            return IsExisted("select * from " + table + " where " + key + "='" + ToInputString(value) + "'");
        }

        internal int MaxValue(string table, string key = "id")
        {
            string sqlString = "select " + key + " from " + table + " order by " + key + " desc";
            _Comm.CommandText = sqlString;
            object result = _Comm.ExecuteScalar();
            return result is null ? -1 : Convert.ToInt32(result);
        }

        private void AddOperationLog(string table, Operation operation)
        {
            Dictionary<string, object> items = new Dictionary<string, object>()
            {
                /*{"sid", _Staff.Items.SID },*/
                {"sid", "0" },
                {"Target", table },
                {"Operation", (int)operation }
            };
            string sqlString = "insert into log_operation " + ToInsertString(items);
            Run(sqlString, Operation.RUN);
        }

        internal bool Run(string sql, Operation operation = Operation.RUN, string table = "")
        {
            _Comm.Reset();
            _Comm.CommandText = sql;
            //Console.WriteLine(sql);
            try
            {
                _Comm.ExecuteNonQuery();
                if (table != "")
                    AddOperationLog(table, operation);
                return true;
            }
            catch (Exception e)
            {
                _Logger.Log(sql, e.Message);
                return false;
            }
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
            Run(sql, Operation.DELETE, tableName);
        }

        private void ClearTable(string tableName)
        {
            Run("delete from " + tableName, Operation.DELETE, tableName);
        }

        #endregion

        #region TypeConvert

        private Dictionary<string, T> ToAvailableItems<T>(Dictionary<string, T> items)
        {
            return items.Where(item => item.Key.ToLower()!="id" && item.Value != null && !string.IsNullOrEmpty(item.Value.ToString()))
                .ToDictionary(item => item.Key, item => item.Value);
        }

        private Dictionary<string, string> ToAvailableInputItems<T>(Dictionary<string, T> items)
        {
            return ToAvailableItems(items).ToDictionary(item => item.Key, item => ToInputString(item.Value));
        }

        private Dictionary<string, string> ToAvailableOutputItems<T>(Dictionary<string, T> items)
        {
            return ToAvailableItems(items).ToDictionary(item => item.Key, item => ToOutputString(item.Value));
        }

        private string ToUpdateString<T>(Dictionary<string, T> items)
        {
            items = ToAvailableItems<T>(items);
            string result = string.Empty;
            foreach (KeyValuePair<string, T> item in items)
            {
                result += "," + item.Key + "='" + ToInputString(item.Value) + "'";
            }
            if (result == "")
                return result;
            else
                return result.Substring(1);
        }

        private string ToInsertString<T>(Dictionary<string, T> items)
        {
            Dictionary<string, string> availableItems = ToAvailableInputItems<T>(items);
            return "(" + String.Join(",", availableItems.Keys.ToArray()) + ") values ('" +
                String.Join("','", availableItems.Values.ToArray()) + "')";
        }

        public static string ToInputString(object obj)
        {
            if (obj == null) return string.Empty;
            Type type = obj.GetType();
            /*if (objString.EndsWith("00:00:00"))
                objString = objString.Substring(0, objString.Length - 9);*/
            if (type == typeof(DateTime))
                return ((DateTime)obj).ToString("s");
            else if (type == typeof(bool))
                return ((bool)obj) ? "1" : "0";
            else
                return obj.ToString().Replace("'", "''");
        }

        public static string ToOutputString(object obj)
        {
            if (obj == null) return string.Empty;
            Type type = obj.GetType();
            if (type == typeof(DateTime))
                return ((DateTime)obj).ToString("yyyy年mm月dd日");
            else if (type == typeof(bool))
                return ((bool)obj) ? "爱" : "不爱";
            else
                return obj.ToString();
        }

        #endregion

    }

}
