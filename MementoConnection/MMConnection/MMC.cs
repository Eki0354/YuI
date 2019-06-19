using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using IOExtension;
using MementoConnection.ELiteItem;

namespace MementoConnection
{
    public enum Operation
    {
        RUN,
        INSERT,
        UPDATE,
        DELETE,
        CREATE
    }

    public partial class MMConnection
    {
        static SQLiteConnection Conn;

        static MMConnection()
        {
            Init();
            //InitLogTable_LogIn();
        }

        #region BASE
        
        public static void Init()
        {
            Conn = new SQLiteConnection();
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder()
            {
                DataSource = MementoPath.MainDataBasePath,
                Password = ""
            };
            Conn.ConnectionString = builder.ConnectionString;
            Conn.Open();
        }

        public static void Close()
        {
            if (Conn != null) Conn.Close();
            Conn = null;
        }

        /// <summary> 扩展类，自动备份文件至目录下的Backup文件夹(不存在将自动新建)。 </summary>
        public static void Backup(this SQLiteConnection conn)
        {
            string sourceFilePath = MementoPath.DataBaseDirectory;
            string destFileDirectoryPath = Path.GetDirectoryName(sourceFilePath) + @"\Backup";
            string destFilePath = destFileDirectoryPath +
                DateTime.Now.ToString("yyyyMMdd HHmmss.fff") + ".bk";
            if (!Directory.Exists(destFileDirectoryPath))
                Directory.CreateDirectory(destFileDirectoryPath);
            File.Copy(sourceFilePath, destFilePath, true);
        }

        public static void Bat(Action action)
        {
            SQLiteTransaction tran = Conn.BeginTransaction();
            action();
            tran.Commit();
        }

        #endregion

        #region BaseOperation_Existed

        /// <summary> 判断返回记录是否为Null空类型。 </summary>
        public static bool IsExisted(string sqlString) =>
            (new SQLiteCommand(sqlString, Conn)).ExecuteScalar() != null;

        /// <summary> 判断符合条件的所有记录数是否为0。 </summary>
        public static bool IsExisted(
            string table, string condition) =>
            Count(table, condition) > 0;

        /// <summary> 判断符合条件的所有记录数。 </summary>
        public static int Count(string sqlString) =>
            ((new SQLiteCommand(sqlString, Conn)).ExecuteScalar() as int?) ?? -1;
            
        /// <summary> 判断符合条件的所有记录数。 </summary>
        public static int Count(
            string table, string condition) =>
            Count(string.Format("select count(*) from {0} where {1}",
                table,
                condition));
        
        #endregion

        #region BaseOperation
        
        private static void AddOperationLog(string table, Operation operation)
        {
            Dictionary<string, object> items = new Dictionary<string, object>()
            {
                /*{"sid", _Staff.Items.SID },*/
                {"sid", 0 },
                {"Target", table },
                {"Operation", operation }
            };
            Execute("insert into log_operation " + items.ParameterInsertString());
        }

        private static bool Execute(string sql,
            Operation operation = Operation.RUN, string table = "log_operation")
        {
            try
            {
                SQLiteCommand comm = new SQLiteCommand(sql, Conn);
                //Console.WriteLine(sql);
                comm.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (table != "log_operation") AddOperationLog(table, operation);
            }
        }

        public static bool Execute(
            string sqlString, Dictionary<string, object> items,
            Operation operation = Operation.RUN, string table = "log_operation")
        {
            try
            {
                SQLiteCommand comm = new SQLiteCommand(sqlString, Conn);
                if (items != null && items.Count > 0)
                    comm.Parameters.AddRange(items.ToParameterArray());
                comm.ExecuteNonQuery();
                Console.WriteLine(comm.CommandText);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (table != "log_operation") AddOperationLog(table, operation);
            }
        }

        public static bool Execute(Dictionary<string, object> items, 
            Operation operation = Operation.RUN,
            string table = "log_operation", string conditon = "")
        {
            string sqlString = "";
            switch (operation)
            {
                case Operation.INSERT:
                    sqlString = "Insert into " + table + " " + items.ParameterInsertString();
                    break;
                case Operation.UPDATE:
                    sqlString = "Update " + table + " set " + items.ParameterUpdateString();
                    if (!string.IsNullOrEmpty(conditon)) sqlString += " where " + conditon;
                    break;
                case Operation.DELETE:
                    sqlString = "Delete from " + table;
                    if (!string.IsNullOrEmpty(conditon)) sqlString += " where " + conditon;
                    break;
                case Operation.CREATE:
                case Operation.RUN:
                    throw new Exception("此方法不支持此操作[Create or Run]");
            }
            return Execute(sqlString, items, operation, table);
        }

        #endregion

        #region ValueOperation

        /// <summary> 返回指定表中指定字段的最大值，返回值为可空值类型。 </summary>
        public static T? MinValue<T>(string table, string key = "id", string condition = "") where T : struct
        {
            string sqlString = "select min(" + key + ") from " + table;
            if (!string.IsNullOrEmpty(condition)) sqlString += " where " + condition;
            return (new SQLiteCommand(sqlString, Conn)).ExecuteScalar() as T?;
        }

        /// <summary> 返回指定表中指定字段的最大值，返回值为可空值类型。 </summary>
        public static T? MaxValue<T>(string table, string key = "id", string condition = "") where T : struct
        {
            string sqlString = "select max(" + key + ") from " + table;
            if (!string.IsNullOrEmpty(condition)) sqlString += " where " + condition;
            return (new SQLiteCommand(sqlString, Conn)).ExecuteScalar() as T?;
        }

        /// <summary> 返回指定格式的字段记录。 </summary>
        public static List<T> GetItems<T>(string table, string keyName, string condition = "")
        {
            string sqlString = "select " + keyName + " from " + table;
            if (!String.IsNullOrEmpty(condition)) sqlString += " where " + condition;
            return GetItems<T>(sqlString);
        }

        /// <summary> 返回指定格式的字段记录。 </summary>
        public static List<T> GetItems<T>(string sqlString)
        {
            DataTable dt = Select(sqlString);
            List<T> items = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                items.Add((T)(row[0]));
            }
            return items;
        }

        /// <summary> 返回指定格式的字段记录。 </summary>
        public static T? GetItem<T>(string table, string keyName, string condition = "") where T : struct
        {
            string sqlString = "select " + keyName + " from " + table;
            if (!String.IsNullOrEmpty(condition)) sqlString += " where " + condition;
            return GetItem<T>(sqlString);
        }

        /// <summary> 返回指定格式的字段记录。 </summary>
        public static T? GetItem<T>(
            string sqlString) where T : struct =>
            (new SQLiteCommand(sqlString, Conn).ExecuteScalar()) as T?;

        /// <summary> 返回指定格式的字段记录。 </summary>
        public static string GetString(string table, string keyName, string condition = "")
        {
            string sqlString = "select " + keyName + " from " + table;
            if (!String.IsNullOrEmpty(condition)) sqlString += " where " + condition;
            return GetString(sqlString);
        }

        /// <summary> 返回指定格式的字段记录。 </summary>
        public static string GetString(string sqlString) =>
            (new SQLiteCommand(sqlString, Conn).ExecuteScalar()) as string;

        #endregion

        #region MainOperation

        public static SQLiteDataReader Read(string sql) =>
            (new SQLiteCommand(sql, Conn)).ExecuteReader();

        /// <summary> 基础查询语句，无记录时返回Null空值。 </summary>
        /// <param name="sql">主查询语句。若此语句中已标识查询限制记录数，则后两个参数无效</param>
        /// <param name="isLimited">标识是否限制查询的最大记录数，默认限制</param>
        /// <param name="limitNum">标识需要限制查询的最大记录数，默认为100条</param>
        /// <returns></returns>
        public static DataTable Select(string sql, bool isLimited = true, int limitNum = 100)
        {
            string tableName = "Too close\r\nToo good to be true.";
            if (isLimited && !sql.Contains("limit")) sql += " limit " + limitNum;
            SQLiteDataAdapter da = new SQLiteDataAdapter(sql, Conn);
            DataSet ds = new DataSet();
            da.Fill(ds, tableName);
            return ds.Tables[tableName];
        }

        public static bool Insert(
            string table, Dictionary<string, object> items) =>
            Execute(items, Operation.INSERT, table);

        public static bool ExecuteUpdate(
            string table, Dictionary<string, object> items, string condition = "") =>
            Execute(items, Operation.UPDATE, table, condition);

        public static bool Delete(
            string table, string condition = "") =>
            Execute(null, Operation.DELETE, table, condition);

        #endregion

        #region TABLES

        private static bool IsExistTable(string name) =>
            IsExisted("select name from sqlite_master where name='" + name + "'");

        private static bool DeleteTable(string tableName) =>
            Execute("drop table " + tableName, Operation.DELETE, tableName);

        private static bool ClearTable(string tableName) =>
            Execute(null, Operation.DELETE, tableName);

        private static bool CreateTable(string tableName, string[] columns = null)
        {
            string sql = "create table [" + tableName + "](\r\n[id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE";
            if (columns != null && columns.Length > 0)
                sql += ",\r\n" + String.Join(",\r\n", columns);
            sql += ")";
            return Execute(sql, Operation.CREATE, tableName);
        }

        #endregion

        #region ELiteDBItem

        public static bool Insert<T>(
            T item) where T : ELiteDBItemBase =>
            Insert(item.TableName, item);

        #endregion

    }
}
