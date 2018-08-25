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
        #region PROPERTY

        private SQLiteConnection _Conn;
        private SQLiteCommand _Comm;
        private SQLiteDataReader _DataReader;
        private string _Path;
        private string _DBPath => _Path + "\\database";
        private string _Name;
        private string _Extension;
        public string FullName => _Name + "." + _Extension;
        public string FullPath => _DBPath + "\\" + FullName;
        private string _BackUpPath => _Path + "\\backup";
        private string _Password;
        private Logger _Logger;
        private EXmlReader _XmlReader;
        private StaffItem _Staff;

        #endregion
        
        #region BASE

        public ELiteConnection(string fullPath = "", string password = "")
        {
            InitializePath(fullPath);
            _Password = password;
            Open();
            InitializeLogger();
            InitializeXmlReader();
            InitializeChannels();
        }

        public ELiteConnection(string path, string name = "min",
            string extension = "db", string password = "")
        {
            InitializePath(path, name, extension);
            _Password = password;
            Open();
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
            _Logger = new Logger(_DBPath, FullName);
            _Logger.Open();
        }

        private void InitializeXmlReader()
        {
            _XmlReader = new EXmlReader(_DBPath, FullName);
            _XmlReader.Open();
        }

        #endregion

        #region STAFF

        public void SetStaff(StaffItem staff)
        {
            if (_Staff != staff) _Staff = staff;
        }

        #endregion

        #region OPERATIONS_BASE

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
        
        public bool IsExisted(string sqlString)
        {
            _Comm.CommandText = sqlString;
            return _Comm.ExecuteNonQuery() > 0;
        }

        public bool IsExisted(string table, string condition)
        {
            return IsExisted("select * from " + table + " where " + condition);
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
            _Comm.Reset();
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

        public static string ItemToString(object obj)
        {
            string objString = null;
            if (obj != null)
            {
                Type type = obj.GetType();
                if (type == typeof(DateTime))
                {
                    objString = ((DateTime)obj).ToString("yyyy-MM-dd HH:mm:ss");
                    if (objString.EndsWith("00:00:00"))
                        objString = objString.Substring(0, objString.Length - 9);
                }
                else if (type == typeof(bool))
                {
                    objString = ((bool)obj) ? "1" : "0";
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
