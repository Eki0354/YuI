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
        
    }
}
