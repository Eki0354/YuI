using System.Collections.Generic;
using System.Data.SQLite;

namespace MementoConnection
{
    public static class KeyValuePairExtension
    {
        public static SQLiteParameter ToSQLiteParameter(
            this KeyValuePair<string, object> pair) => 
            new SQLiteParameter(pair.Key, pair.Value);

        public static string ToParameterEqualString(
            this KeyValuePair<string, object> pair, string equalStr = "=") =>
            pair.Key + equalStr + "@" + pair.Key;
        
    }
}
