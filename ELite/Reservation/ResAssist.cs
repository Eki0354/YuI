using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ELite;

namespace ELite.Reservation
{
    public class ResAssist
    {
        ELiteConnection _Conn;

        public ResAssist(ELiteConnection conn)
        {
            _Conn = conn;
        }

        public List<ELiteListBoxResItem> UncheckedResList()
        {
            DataTable dt = _Conn.SelectUncheckedRes();
            List<ELiteListBoxResItem> resList = _Conn.FromDataTable<ELiteListBoxResItem>(dt);
            return resList;
        }
    }
}
