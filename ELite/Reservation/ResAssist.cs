﻿using System;
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

        public List<ListBoxResItem> UncheckedResList()
        {
            DataTable dt = _Conn.SelectUncheckedRes();
            List<ListBoxResItem> resList = new List<ListBoxResItem>();
            if (dt.Rows.Count < 1) return resList;
            foreach (DataRow row in dt.Rows)
            {
                resList.Add(new ListBoxResItem(ELiteConnection.Channels.Find(
                    c => c.ID == Convert.ToInt32(row[0])).Title_en_us, row[1].ToString()));
            }
            return resList;
        }
    }
}