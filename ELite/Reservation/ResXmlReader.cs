using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELite.Reservation
{
    public enum ChannelSniffMethod
    {
        CLIPBOARD,
        NET,
        LOCAL,
        ERROR
    }

    public class ResXmlReader : EXmlReader
    {
        public ResXmlReader(string fullPath) : base(fullPath)
        {

        }

        public ResXmlReader(string path, string name = "ResConfig", string extension = "xml") : base(path, name, extension)
        {

        }

        public ChannelSniffMethod GetSniffMethod(string channel)
        {
            if (!int.TryParse(ReadValue(channel + "/SniffMethod"), out int methodValue)) return ChannelSniffMethod.ERROR;
            if (methodValue >= (int)ChannelSniffMethod.ERROR || methodValue < 0) return ChannelSniffMethod.ERROR;
            return (ChannelSniffMethod)methodValue;
        }

    }
}
