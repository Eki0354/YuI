using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledros.Json
{
    public class JsonBookingSpark
    {
        public class RootObject
        {
            public string Type { get; set; }
            public string Channel { get; set; }
            public string ResNumber { get; set; }
            public string Cookie { get; set; }
        }
    }
}
