using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace MementoConnection
{
    public static class DictionaryExtension
    {
        public static List<SQLiteParameter> ToParameterList(this Dictionary<string, object> items)
        {
            List<SQLiteParameter> parameters = new List<SQLiteParameter>();
            foreach (KeyValuePair<string, object> item in items) 
            {
                parameters.Add(item.ToSQLiteParameter());
            }
            return parameters;
        }

        public static SQLiteParameter[] ToParameterArray(
            this Dictionary<string, object> items) => items.ToParameterList().ToArray();

        public static string ParameterInsertString(this Dictionary<string, object> items)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" (");
            sb.Append(String.Join(",", items.Keys));
            sb.Append(") values (@");
            sb.Append(String.Join(",@", items.Keys));
            sb.Append(")");
            return sb.ToString();
        }

        public static string ParameterUpdateString(this Dictionary<string, object> items)
        {
            List<string> itemStrs = new List<string>();
            foreach (KeyValuePair<string, object> item in items)
            {
                itemStrs.Add(item.ToParameterEqualString());
            }
            return String.Join(",", itemStrs.ToArray());
        }

        public static string ParameterConditionAndOnlyString(this Dictionary<string, object> items)
        {
            List<string> itemStrs = new List<string>();
            foreach (KeyValuePair<string, object> item in items)
            {
                itemStrs.Add(item.ToParameterEqualString());
            }
            return String.Join(" and ", itemStrs.ToArray());
        }
    }
}
