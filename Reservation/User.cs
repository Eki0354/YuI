using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoomCell;

namespace Reservation
{
    public enum MaritalStates : byte { Unmarried}
    public class User
    {
        //账户信息
        public string UID { get; }
        string _account;
        string _password;
        string _saltValue;
        string _englishName { get; }
        string _chineseName { get; }
        public string SurName(bool InChinese = true) { return PickName(InChinese ? _chineseName : _englishName, "<<", true); }
        public string GivenNames(bool InChinese = true) { return PickName(InChinese ? _chineseName : _englishName, "<<", false); }
        public string FirstName(bool InChinese = true) { return PickName(SurName(InChinese), "<", true); }
        public string LastName(bool InChinese = true) { return PickName(GivenNames(InChinese), "<", false); }
        public string FullName(bool InChinese = true) { return InChinese ? _chineseName : _englishName; }
        string PickName(string name, string separator, bool fore)
        {
            if (name.IndexOf(separator) == -1)
            {
                return name;
            }
            else
            {
                if (fore)
                {
                    return name.Substring(0, name.IndexOf(separator));
                }
                else
                {
                    return name.Substring(name.IndexOf(separator) + 1);
                }
            }
        }
        public Guest ToGuest() { return new Guest(UID, _englishName, _chineseName); }
        //个性化信息
        public string NickName { get; }
        public DateTime Birth { get; }
        public byte PreferredLanguage { get; }
        public byte AreaFrom { get; }
        public string EmailAddress { get; }
        public string PhoneNumber { get; }
        public byte Sex { get; }
        public byte MaritalStatus { get; }
        public string Descriptions { get; }
    }
}
