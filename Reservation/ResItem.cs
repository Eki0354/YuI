using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reservation
{
    public enum ResStatus
    {
        OK,
        CHANGED,
        CANCELLED
    }

    public class ItemCollection
    {
        protected Dictionary<string, object> _Items;

        public ItemCollection(List<string> items) : base()
        {
            items.ForEach(item => _Items.Add(item, null));
        }

        public ItemCollection(Dictionary<string, object> items) : base()
        {
            _Items = items;
        }

        public void Add(string key, object value)
        {
            _Items.Add(key, value);
        }

        public object Get(string key)
        {
            if (!_Items.ContainsKey(key)) return null;
            return _Items[key];
        }

        public bool ContainsKey(string key)
        {
            return _Items.ContainsKey(key);
        }
    }

    public class ResRoomItemCollection : ItemCollection
    {
        public int UID
        {
            get { return (int)_Items["UID"]; }
        }
        public string Type { get; set; }
        public DateTime ReservedDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public float Price { get; set; }
        public int Persons { get; set; }
        public int Rooms { get; set; }
        public int Night { get; set; }
        public ResStatus Status { get; set; }

        public ResRoomItemCollection():base(new List<string>()
        {
            "UID",
            "Type",
            "ReservedDate",
            "ArrivalDate",
            "DepartureDate",
            "Price",
            "Persons",
            "Rooms",
            "Night",
            "Status"
        })
        {

        }

    }

    public struct ResItemCollection
    {
        public bool Checked { get; set; }
        public string ResNumber { get; set; }
        public int UID { get; set; }
        public int Channel { get; set; }
        public string ChannelLanguage { get; set; }
        public float Deposit { get; set; }
        public float Subtotal { get; set; }
        public float Balance { get; set; }
        public DateTime BookedDateTime { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int Persons { get; set; }
        public int Rooms { get; set; }
        public int Nights { get; set; }
        public string Remarks { get; set; }
        public int LID { get; set; }
    }

    public class Booking
    {
        public ResItemCollection ResItem = new ResItemCollection();
        public List<ResRoomItemCollection> ResRoomItemList = new List<ResRoomItemCollection>();

        public Booking()
        {

        }

        private Dictionary<string,object> GetResItemDict()
        {
            return new Dictionary<string, object>()
            {
                {"Checked",ResItem.Checked  },
                {"ResNumber",ResItem.ResNumber },
                {"UID",ResItem.UID },
                {"Channel",ResItem.Channel },
                {"ChannelLanguage",ResItem.ChannelLanguage },
                {"Deposit",ResItem.Deposit },
                {"Subtotal",ResItem.Subtotal },
                {"Balance",ResItem.Balance },
                {"BookedDateTime",ResItem.BookedDateTime },
                {"ArrivalDate",ResItem.ArrivalDate },
                {"DepartureDate",ResItem.DepartureDate },
                {"ArrivalTime",ResItem.ArrivalTime },
                {"Persons",ResItem.Persons },
                {"Rooms",ResItem.Rooms },
                {"Nights",ResItem.Nights },
                {"Remarks",ResItem.Remarks },
                {"LID",ResItem.LID }
            };
        }

        public List<Dictionary<string,object >> GetRoomItemDictList()
        {
            return null;
        }
    }
}
