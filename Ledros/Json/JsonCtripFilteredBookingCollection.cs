using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledros.Json
{
    public class JsonCtripFilteredBookingCollection
    {
        public class Data
        {
            public string FormID { get; set; }
            public string OrderKey { get; set; }
            public string SourceType { get; set; }
            public string SourceTypeDisplay { get; set; }
            public string OrderID { get; set; }
            public string OrderType { get; set; }
            public string OrderTypeDisplay { get; set; }
            public string OrderStatus { get; set; }
            public string OrderStatusDisplay { get; set; }
            public string FormDate { get; set; }
            public string FormDateOriginal { get; set; }
            public string RoomName { get; set; }
            public string RoomEnName { get; set; }
            public string RoomNameDisplay { get; set; }
            public string IsUrgent { get; set; }
            public string Quantity { get; set; }
            public string QuantityDisplay { get; set; }
            public string Arrival { get; set; }
            public string Departure { get; set; }
            public string ArrivalAndDeparture { get; set; }
            public string LiveDays { get; set; }
            public string ClientName { get; set; }
            public string ClientNameDisplay { get; set; }
            public string Token { get; set; }
            public string IsRiskyOrder { get; set; }
            public string RiskyType { get; set; }
            public string Hotel { get; set; }
            public string HotelName { get; set; }
            public string IsShowHotelName { get; set; }
            public string IsCountDownOrder { get; set; }
            public string CountDownSeconds { get; set; }
            public string CountDownSequence { get; set; }
            public string OutcallTime { get; set; }
            public string IsHastened { get; set; }
            public string IsAutoConfirmed { get; set; }
            public string IsPreModifyNotify { get; set; }
            public string IsCON { get; set; }
            public string IsGuaranteed { get; set; }
            public string IsHoldRoom { get; set; }
            public string GuaranteeType { get; set; }
            public string IsPP { get; set; }
            public string IsFG { get; set; }
            public string PaymentType { get; set; }
            public string AllicanceName { get; set; }
            public string IsHourRoom { get; set; }
            public string UrgeCount { get; set; }
            public string IsFullyBookedOrder { get; set; }
            public string GroupOrderType { get; set; }
        }

        public class RootObject
        {
            public string Rcode { get; set; }
            public string Msg { get; set; }
            public string Script { get; set; }
            public List<Data> Data { get; set; }
            public string TotalPage { get; set; }
            public string TotalRecords { get; set; }
        }
    }
}
