using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace RoomCell
{
    public class Guest
    {
        public string UID { get; }
        string _englishName;
        string _chineseName;
        public Guest()
        {

        }
        public Guest(string uid, string englishName, string chineseName)
        {
            UID = uid;
            _englishName = englishName;
            _chineseName = chineseName;
        }
        public string FullName(bool InChinese = true) { return InChinese ? _chineseName : _englishName; }
    }
}
