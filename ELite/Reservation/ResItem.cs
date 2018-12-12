using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ELite;

namespace ELite.Reservation
{
    public static class RItemKeys
    {
        private static string[] _ListBoxResItemKeys = new string[] { "Channel", "ResNumber" };
        public static string[] ListBoxResItemKeys => _ListBoxResItemKeys;
        private static string[] _ResItemKeys = new string[]
        {
            "id",
            "Checked",
            "ResNumber",
            "uid",
            "Channel",
            "ChannelLanguage",
            "Deposit",
            "Subtotal",
            "Balance",
            "BookedDateTime",
            "ArrivalDate",
            "DepartureDate",
            "ArrivalTime",
            "Persons",
            "Nights",
            "Rooms",
            "Remarks",
            "Status",
            "LastChangedDateTime",
            "RecordedDateTime",
            "lid"
        };
        public static string[] ResItemKey => _ResItemKeys;
        private static string[] _ResRoomItemKeys = new string[]
        {
            "id",
            "ResNumber",
            "uid",
            "Type",
            "ReservedDate",
            "ArrivalDate",
            "DepartureDate",
            "Price",
            "Persons",
            "Rooms",
            "Nights",
            "Status"
        };
        public static string[] ResRoomItemKeys => _ResRoomItemKeys;
        private static string[] _ResUserItemKeys = new string[]
        {
            "uid",
            "Surname",
            "MiddleName",
            "GivenName",
            "Country",
            "Email",
            "Phone",
            "Identity",
            "PreferredLanguage",
            "Address"
        };
        public static string[] ResUserItemKeys => _ResUserItemKeys;
    }

    public enum ResStatus
    {
        OK,
        CHANGED,
        CANCELLED
    }

    #region DEFINE: DBItem

    /// <summary> 产生Reservation中ListBoxResItem、ResItem、ResRoomItem、ResUserItem的基类。 </summary>
    public class DBItem
    {
        protected Dictionary<string, object> _Dict = new Dictionary<string, object>();

        public DBItem(string[] keys = null)
        {
            foreach(string key in keys)
            {
                _Dict.Add(key, null);
            }
        }

        public DBItem(object[] objs, string[] keys)
        {
            for(int i = 0; i < objs.Length; i++)
            {
                _Dict.Add(keys[i], objs[i]);
            }
        }

        public DBItem(Dictionary<string, object> source, string[] keys)
        {
            foreach (string key in keys)
            {
                _Dict.Add(key, null);
            }
            foreach (KeyValuePair<string, object> item in source)
            {
                if (!_Dict.ContainsKey(item.Key)) continue;
                _Dict[item.Key] = item.Value;
            }
        }

        public object GetValue(string key)
        {
            if (!_Dict.ContainsKey(key)) return null;
            return _Dict[key];
        }

        public override string ToString()
        {
            if (_Dict == null || _Dict.Count < 1) return null;
            string result = string.Empty;
            foreach (KeyValuePair<string, object> pair in _Dict) 
            {
                result += "\r\n" + pair.Key + ": " + ELiteConnection.ToInputString(pair.Value);
            }
            return result.Substring(2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Type objType = obj.GetType();
            if (objType != this.GetType()) return false;
            if (objType == typeof(DBItem))
                return ((DBItem)obj)._Dict.SequenceEqual(_Dict);
            else if (objType == typeof(DBResItem))
                return ((DBResItem)obj)._Dict.SequenceEqual(_Dict);
            else if (objType == typeof(DBResRoomItem))
                return ((DBResRoomItem)obj)._Dict.SequenceEqual(_Dict);
            else if (objType == typeof(DBResUserItem))
                return ((DBResUserItem)obj)._Dict.SequenceEqual(_Dict);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary> 返回格式：'(key1,key2...) values (value1,value2...)'，仅保留有效值。 </summary>
        public string ToInsertString()
        {
            Dictionary<string, string> strDict = ToStringDictionary();
            if (strDict == null || strDict.Count < 1) return string.Empty;
            return "(" + String.Join(",", strDict.Keys.ToArray()) + ") values ('"
                + String.Join(",", strDict.Values.ToArray()) + "')";
        }
        
        public Dictionary<string, string> ToStringDictionary()
        {
            if (_Dict == null || _Dict.Count < 1) return null;
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (KeyValuePair<string, object> item in _Dict)
            {
                string valueStr = ELiteConnection.ToInputString(item.Value);
                if (String.IsNullOrEmpty(valueStr)) continue;
                result.Add(item.Key, valueStr);
            }
            return result;
        }

        public DBResItem ToResItem() { return new DBResItem(_Dict); }

        public DBResRoomItem ToRoomItem() { return new DBResRoomItem(_Dict); }

        public DBResUserItem ToUserItem() { return new DBResUserItem(_Dict); }
    }

    /// <summary> 继承自RItem的User类，用于表示与某订单绑定的单一用户。 </summary>
    public class DBResUserItem : DBItem
    {
        #region PROPERTY

        public int UID => Convert.ToInt32(_Dict["uid"]);
        public string Surname => Convert.ToString(_Dict["Surname"]);
        public string MiddleName => Convert.ToString(_Dict["MiddleName"]);
        public string GivenName => Convert.ToString(_Dict["GivenName"]);
        public string FullName
        {
            get
            {
                string fullName = string.Empty;
                foreach (string name in new string[] { Surname, MiddleName, GivenName })
                {
                    if (String.IsNullOrEmpty(name)) continue;
                    fullName += " " + name;
                }
                return fullName.Substring(1);
            }
        }
        public string Country => Convert.ToString(_Dict["Country"]);
        public string Email => Convert.ToString(_Dict["Email"]);
        public string Phone => Convert.ToString(_Dict["Phone"]);
        public string Identity => Convert.ToString(_Dict["Identity"]);
        public string PreferredLanguage => Convert.ToString(_Dict["PreferredLanguage"]);
        public string Address => Convert.ToString(_Dict["Address"]);

        #endregion

        public DBResUserItem():base(RItemKeys.ResUserItemKeys)
        {

        }

        public DBResUserItem(object[] objs) : base(objs, RItemKeys.ResUserItemKeys)
        {

        }

        public DBResUserItem(Dictionary<string, object> dict) : base(dict, RItemKeys.ResUserItemKeys)
        {

        }
    }
    
    /// <summary> 继承自RItem的Room类，通常使用此类的泛型，用于表示订单中的房型集合。 </summary>
    public class DBResRoomItem : DBItem
    {
        #region PROPERTY
        
        public int ID => Convert.ToInt32(_Dict["id"]);
        public string ResNumber => Convert.ToString(_Dict["ResNumber"]);
        public int UID => Convert.ToInt32(_Dict["uid"]);
        public string Type => Convert.ToString(_Dict["Type"]);
        public DateTime ReservedDate => Convert.ToDateTime(_Dict["ReservedDate"]);
        public DateTime ArrivalDate => Convert.ToDateTime(_Dict["ArrivalDate"]);
        public DateTime DepartureDate => Convert.ToDateTime(_Dict["DepartureDate"]);
        public float Price => Convert.ToSingle(_Dict["Price"]);
        public int Persons => Convert.ToInt32(_Dict["Persons"]);
        public int Rooms => Convert.ToInt32(_Dict["Rooms"]);
        public int Nights => Convert.ToInt32(_Dict["Nights"]);
        public ResStatus Status => (ResStatus)(Convert.ToInt32(_Dict["Status"]));

        #endregion

        public DBResRoomItem() : base(RItemKeys.ResRoomItemKeys)
        {
            
        }

        public DBResRoomItem(object[] objs) : base(objs, RItemKeys.ResRoomItemKeys)
        {
            
        }

        public DBResRoomItem(Dictionary<string, object> dict) : base(dict, RItemKeys.ResRoomItemKeys)
        {

        }
    }

    /// <summary> 继承自RItem的Res类，用于表示与某订单绑定的属性集合。 </summary>
    public class DBResItem : DBItem
    {
        #region PROPERTY
        
        public int ID => Convert.ToInt32(_Dict["id"]);
        public bool Checked => Convert.ToBoolean(_Dict["Checked"]);
        public string ResNumber => Convert.ToString(_Dict["ResNumber"]);
        public int UID => Convert.ToInt32(_Dict["uid"]);
        public int Channel => Convert.ToInt32(_Dict["Channel"]);
        public string ChannelLanguage => Convert.ToString(_Dict["ChannelLanguage"]);
        public float Deposit => Convert.ToSingle(_Dict["Deposit"]);
        public float Subtotal => Convert.ToSingle(_Dict["Subtotal"]);
        public float Balance => Convert.ToSingle(_Dict["Balance"]);
        public DateTime BookedDateTime => Convert.ToDateTime(_Dict["BookedDateTime"]);
        public DateTime ArrivalDate => Convert.ToDateTime(_Dict["ArrivalDate"]);
        public DateTime DepartureDate => Convert.ToDateTime(_Dict["DepartureDate"]);
        public string ArrivalTime => Convert.ToString(_Dict["ArrivalTime"]);
        public int Persons => Convert.ToInt32(_Dict["Persons"]);
        public int Rooms => Convert.ToInt32(_Dict["Rooms"]);
        public int Nights => Convert.ToInt32(_Dict["Nights"]);
        public string Remarks => Convert.ToString(_Dict["Remarks"]);
        public int Status => Convert.ToInt32(_Dict["Status"]);
        public DateTime LastChangedDateTime => Convert.ToDateTime(_Dict["LastChangedDateTime"]);
        public DateTime RecordedDateTime => Convert.ToDateTime(_Dict["RecordedDateTime"]);
        public int LID => Convert.ToInt32(_Dict["lid"]);

        #endregion

        public DBResItem() : base(RItemKeys.ResItemKey)
        {
            
        }

        public DBResItem(object[] objs) : base(objs, RItemKeys.ResItemKey)
        {
            
        }

        public DBResItem(Dictionary<string, object> dict) : base(dict, RItemKeys.ResItemKey)
        {

        }

        public ELiteListBoxResItem ListBoxResItem => new ELiteListBoxResItem()
        {
            Channel = ELiteConnection.Channels.Find(c => c.ID == Channel).Title_en_us,
            ResNumber = ResNumber,
            FullName = ELiteConnection.DefaultUserName
        };

    }
    
    #endregion

    /// <summary> 订单基类 </summary>
    public class Booking
    {
        public static EXmlReader _XmlReader;
        public DBResItem ResItem { get; set; }
        public DBResUserItem ResUserItem { get; set; }
        public List<DBResRoomItem> ResRoomItemList { get; set; }

        public Booking()
        {
            ResItem = new DBResItem();
            ResUserItem = new DBResUserItem();
            ResRoomItemList = new List<DBResRoomItem>();
            if (_XmlReader != null) return;
            _XmlReader = new EXmlReader(
                Environment.CurrentDirectory, "booking");
            _XmlReader.Open();
        }

        public Booking(Dictionary<string, object> resItems, Dictionary<string, object> resUserItems,
            List<Dictionary<string, object>> resRoomItemList) : base()
        {
            ResItem = new DBResItem(resItems);
            ResUserItem = new DBResUserItem(resUserItems);
            ResRoomItemList = new List<DBResRoomItem>();
            resRoomItemList.ForEach(resRoomItem => 
                ResRoomItemList.Add(new DBResRoomItem(resRoomItem)));
        }

        public Booking(DBResItem resItem, DBResUserItem resUserItem,
            List<DBResRoomItem> resRoomItemList) : base()
        {
            ResItem = resItem;
            ResUserItem = resUserItem;
            ResRoomItemList = resRoomItemList;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Booking)) return false;
            Booking objBooking = (Booking)obj;
            if (objBooking.ResItem == null || objBooking.ResRoomItemList == null) return false;
            if (objBooking.ResItem != ResItem) return false;
            if (objBooking.ResRoomItemList.Count != ResRoomItemList.Count) return false;
            for(int i = 0; i < ResRoomItemList.Count; i++)
            {
                if (objBooking.ResRoomItemList[i] != ResRoomItemList[i]) return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return ResItem.ToString() + "\r\n" + ResUserItem.ToString();
        }

        public string ToMainInfoString()
        {
            return ResUserItem.FullName + "\r\n\r\n" + ResItem.ResNumber + 
                "\r\n\r\n" + ResUserItem.Email;
        }
        
        private string ToPricesString()
        {
            string result = string.Empty;
            ResRoomItemList.ForEach(item => result += item.Price);
            return "Price: " + result;
        }

        public string ToContentString()
        {
            Dictionary<string, string> resDcit = ResItem.ToStringDictionary();
            Dictionary<string, string> resUserDict = ResUserItem.ToStringDictionary();
            Dictionary<string, string> dpDict = new Dictionary<string, string>()
            {
                {"ResNumber", "Ref" },
                {"Price", "Room Fee" },

            };
            return null;
        }
    }
    
}
