using System.Data.SQLite;
using MementoConnection.ELiteItem;

namespace MementoConnection
{
    public static class SQLiteConnectionExtension
    {
        public static void SQLiteTest(this SQLiteConnection conn)
        {
            ELiteRoomFieldCollection item = new ELiteRoomFieldCollection
            {
                ["RID"] = 421,
                ["Type"] = 0,
                ["Floor"] = 0,
                ["Building"] = 0,
                ["IsDorm"] = false
            };
            //item.InsertTo(conn);
        }
    }
}
