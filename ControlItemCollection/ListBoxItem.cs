using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlItemCollection
{
    public class ListBoxResItem
    {
        public string Channel { get; }
        public string ResNumber { get; }

        public ListBoxResItem(string channel, string resNumber)
        {
            Channel = channel;
            ResNumber = resNumber;
        }

        public override string ToString()
        {
            return Channel + "\r\n" + ResNumber;
        }
    }
}
