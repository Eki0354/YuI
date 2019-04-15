using System.Data;

namespace MementoConnection
{
    public partial class MMConnection
    {
        public static int LoggedInSID { get; set; } = -1;

        public static DataRow GetMaxSIDStaff()
        {
            DataTable dt = Select(
                "select * from info_staff where sid=(select max(sid) from info_staff)");
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public static DataRow LoggedInStaffDataRow =>
            LoggedInSID < 0 ? null : 
            Select(string.Format("select * from info_staff where sid={0}",LoggedInSID)).Rows[0];
    }
}
