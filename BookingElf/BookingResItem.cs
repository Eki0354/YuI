using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookingElf
{

    public class BookingResItem
    {
        public UserInfo UserInfo { get; set; }
        public ResInfo ResInfo { get; set; }
        public List<RoomInfo> RoomInfo { get; set; }

        public static DateTime InitDate(string text)
        {
            //text = new Regex(@"(?<=\w+,\s*)\S[\s\S]+(?=[星期]*)").Match(text).Value.Replace(",", "")
            //    .Replace(" ", "/").Replace("年", "/").Replace("月", "/");
            text = text.Trim();
            bool isCN = int.TryParse(text.Substring(0, 1), out int i);
            text = isCN ? text.Substring(0, text.Length - 4) : text.Substring(5);
            text = text.Replace(",", "").Replace(" ", "/").Replace("年", "/").Replace("月", "/");
            return DateTime.ParseExact(text, isCN?"yyyy/M/d":"MMM/d/yyyy",
                System.Globalization.DateTimeFormatInfo.InvariantInfo,
                System.Globalization.DateTimeStyles.None);
        }
        
        public static string InitNum(string text)
        {
            return new Regex(@"[\d.]+").Match(text.Replace(" ", "").Replace(",", "")).Value;
        }
    }

    public class UserInfo
    {
        public string FullName { get; set; }
        public string Country { get; set; }
        public string PreferredLanguage { get; set; }
        public string Email { get; set; }
        public Dictionary<string,object > ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "FullName", FullName },
                { "Country", Country },
                { "PreferredLanguage", PreferredLanguage },
                { "Email", Email }
            };
        }
    }

    public class ResInfo
    {
        public string SubTotal { get; set; }
        public string Rooms { get; set; }
        public string Persons { get; set; }
        public string BookedDateTime { get; set; }
        public string ResNumber { get; set; }
        public string ChannelLanguage { get; set; }
        public string ArrivalTime { get; set; }
        public string ArrivalDate { get; set; }
        public string DepartureDate { get; set; }
        public Dictionary<string, object> ToDictionary()
        {
            DateTime date = BookingResItem.InitDate(DepartureDate);
            return new Dictionary<string, object>()
            {
                { "SubTotal", BookingResItem.InitNum(SubTotal) },
                { "Rooms", BookingResItem.InitNum(Rooms) },
                { "Persons", BookingResItem.InitNum(Persons) },
                { "BookedDateTime", BookingResItem.InitDate(BookedDateTime) },
                { "ResNumber", ResNumber },
                { "ChannelLanguage", ChannelLanguage },
                { "ArrivalTime", ArrivalTime },
                { "ArrivalDate", BookingResItem.InitDate(ArrivalDate) },
                { "DepartureDate", BookingResItem.InitDate(DepartureDate) },
            };
        }
    }

    public class RoomInfo
    {
        public string Persons { get; set; }
        public string FullName { get; set; }
        public string Type { get; set; }
        public string ArrivalDate { get; set; }
        public string DepartureDate { get; set; }
        public RepeatArraylist RepeatArrayList { get; set; }
        public List<Dictionary<string,object>> ToDictionaryArry()
        {
            var dicts = new List<Dictionary<string, object>>();
            DateTime arrDate = BookingResItem.InitDate(ArrivalDate);
            DateTime depDate = BookingResItem.InitDate(DepartureDate);
            for (int i = 0; i < (depDate - arrDate).Days; i++)
            {
                dicts.Add(new Dictionary<string, object>
                {
                    { "Persons", BookingResItem.InitNum(Persons) },
                    { "FullName", FullName },
                    { "Type", Type },
                    { "ReservedDate", arrDate.AddDays(i) },
                    { "ArrivalDate", arrDate },
                    { "DepartureDate", depDate },
                    { "Price", BookingResItem.InitNum(RepeatArrayList.Price[i]) }
                });
            }
            return dicts;
        }
    }

    public class RepeatArraylist
    {
        public List<string> Price { get; set; }
    }


}
