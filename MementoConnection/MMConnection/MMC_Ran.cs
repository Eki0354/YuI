using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoConnection
{
    public partial class MMConnection
    {
        public static DataTable GetStaffList => Select("select * from info_staff", false);
        
        public static void LogIn(int sid)
        {
            InsertLogInLog(sid);
            LoggedInSID = sid;
        }

        public static bool InsertLogInLog(int sid) =>
            Execute(string.Format("insert into log_staff (sid) values({0})", sid));

    }
}
