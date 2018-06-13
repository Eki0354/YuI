using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EkiDataBase;
using EkiDataBase.ItemSet;
using System.Data;

namespace Reservation
{
    public enum ReservationState : byte { Unacknowledged, Acknowledged, Changed, Cancelled, CheckedIn, CheckedOut, NoShow }
    public enum PaymentState : byte { Unpaid, Deposit, Guarantee, Paid }
    public class ReservationItemCollection : EDBItemCollection
    {
        public List<EDBItemCollection> RoomItems = new List<EDBItemCollection>();
        public ReservationItemCollection(DataRow r = null) : base("info_res", "Order")
        {
            Add(new EDBBooleanItem("Recorded"));
            Add(new EDBBooleanItem("Confirmed"));
            Add(new EDBStringItem("Order"));
            Add(new EDBStringItem("User"));
            Add(new EDBByteItem("Channel"));
            Add(new EDBByteItem("ChannelLanguage"));
            Add(new EDBStringItem("Source"));
            Add(new EDBDateTimeItem("Booked"));
            Add(new EDBDateTimeItem("Arrival"));
            Add(new EDBDateTimeItem("Departure"));
            Add(new EDBByteItem("Persons"));
            Add(new EDBByteItem("Rooms"));
            Add(new EDBByteItem("Nights"));
            Add(new EDBSingleItem("Deposit"));
            Add(new EDBSingleItem("TotalCost"));
            Add(new EDBSingleItem("Balance"));
            Add(new EDBByteItem("State"));
            Add(new EDBStringItem("Remarks"));
            //Add(new EDBByteItem("Country"));
            //Add(new EDBByteItem("PreferredLanguage"));
            //Add(new EDBStringItem("PhoneNumber"));
            //Add(new EDBStringItem("EmailAddress"));
            //Add(new EDBStringItem("ResidenceAddress"));
            //用于读取订单时，传入非空数据行，按列标题赋值
            if (r != null)
            {
                Items.ForEach(x => x.SetValue(r[x.Key()]));
            }
        }
        public void AddRoomItem(EDBItemCollection item)
        {
            RoomItems.Add(item);
        }
    }
}
