using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFElf
{
    public class OutlookEmail
    {
        public string Address { get; }
        public string Theme { get; }
        public string Body { get; }

        public OutlookEmail(string address, string theme, string body)
        {
            this.Address = address;
            this.Theme = theme;
            this.Body = body;
        }
    }
}
