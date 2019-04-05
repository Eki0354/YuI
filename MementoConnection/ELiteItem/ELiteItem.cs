using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Text;
using MMC = MementoConnection.MMConnection;

namespace MementoConnection.ELiteItem
{
    public abstract class ELiteDBItemBase : Dictionary<string, object>
    {
        #region Properties

        public abstract string TableName { get; }
        public abstract string FieldsString { get; }

        public string ParameterInsertString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Insert into ");
                sb.Append(TableName);
                sb.Append(" (");
                sb.Append(String.Join(",", this.Keys));
                sb.Append(") values (@");
                sb.Append(String.Join(",@", this.Keys));
                sb.Append(")");
                return sb.ToString();
            }
        }

        public List<SQLiteParameter> ParameterList
        {
            get
            {
                List<SQLiteParameter> parameters = new List<SQLiteParameter>();
                foreach (KeyValuePair<string, object> item in this)
                {
                    parameters.Add(new SQLiteParameter(item.Key, item.Value));
                }
                return parameters;
            }
        }

        public SQLiteParameter[] ParameterArray => this.ParameterList.ToArray();

        #endregion

        public ELiteDBItemBase()
        {
            foreach(string field in FieldsString.Split(','))
            {
                this.Add(field, null);
            }
        }

        public ELiteDBItemBase(DataRow row)
        {
            foreach (string field in FieldsString.Split(','))
            {
                this.Add(field, row[field]);
            }
        }

        public static T FromDataRow<T>(DataRow row) where T : ELiteDBItemBase =>
            System.Activator.CreateInstance(typeof(T), row) as T;
        
    }

    public static class ELiteDBeItemCollectionHelper
    {

        #region Base

        public static List<string> InsertTo<T>(this ObservableCollection<T> oc,
            SQLiteConnection conn) where T : ELiteDBItemBase
        {
            List<string> stringList = new List<string>();
            for (int i = 0; i < oc.Count; i++)
            {
                MMC.Execute(oc[i].ParameterInsertString, oc[i], Operation.INSERT, oc[i].TableName);
            }
            return stringList;
        }

        public static void InitFromDataTable<T>(this ObservableCollection<T> oc,
            DataTable table) where T : ELiteDBItemBase
        {
            oc.Clear();
            foreach (DataRow row in table.Rows)
            {
                oc.Add(ELiteDBItemBase.FromDataRow<T>(row));
            }
        }

        #endregion

        #region RoomItem
        
        #endregion

    }
}
