using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ledros
{
    public class HeartReader
    {
        static XDocument doc;

        static HeartReader()
        {
            doc = XDocument.Load(Environment.CurrentDirectory + "\\Kalista.xml");
        }

        static string GetString(string type, string method, string channel, string goal)
        {
            var filtered = from p in doc.Descendants("String")
                           where (string)p.Attribute("type") == type &&
                           (string)p.Attribute("method") == method &&
                           (string)p.Attribute("channel") == channel &&
                           (string)p.Attribute("goal") == goal
                           select new
                           {
                               PostString = (string)p.Value
                           };
            return filtered.Count() > 0 ? filtered.First().PostString : null;
        }

        public static string GetPostParam(string channel, string goal) =>
            GetString("Param", "Post", channel, goal);

        public static string GetPostPort(string channel, string goal) =>
            GetString("Port", "Post", channel, goal);
    }
}
