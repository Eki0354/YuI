using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Room
{
    public class RoomFacility
    {
        public Dictionary<string, bool> Items { get; } = new Dictionary<string, bool>();
        public RoomFacility(DataTable facility)
        {
            foreach(DataColumn c in facility.Columns)
            {
                Items.Add(c.Caption, Convert.ToBoolean(facility.Rows[0][c.Caption]));
            }
        }
    }
}
