using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StringDict = System.Collections.Generic.Dictionary<string, string>;
using SODict = System.Collections.Generic.Dictionary<string, object>;
using System.Text.RegularExpressions;

namespace Ledros.Json
{
    public class JsonCtripBooking
    {
        public class DailyAmount
        {
            public string DateDisplay { get; set; }
            public string AmountDisplay { get; set; }
        }

        public class Invoice
        {
            public string Info { get; set; }
            public List<DailyAmount> DailyAmount { get; set; }
        }

        public class PromotionItems
        {
            public string Type { get; set; }
            public string TypeName { get; set; }
            public string TypeNameMultLanguage { get; set; }
            public string Currency { get; set; }
            public string GapValue { get; set; }
            public string GapValueDisplay { get; set; }
            public string SellGapValue { get; set; }
            public string SellGapValueDisplay { get; set; }
            public string CostDiscount { get; set; }
            public string SellDiscount { get; set; }
            public string Remark { get; set; }
            public string TypeNameDisplay { get; set; }
            public string Version { get; set; }
            public string RuleId { get; set; }
            public string RuleName { get; set; }
        }

        public class OrderRoomPrices
        {
            #region Extra Properties

            public SODict ToDictionary(RootObject root)=>
                new SODict
                {
                    { "ResNumber", root.Data.OrderID },
                    { "Type", root.Data.RoomName },
                    { "ReservedDate", DateTime.Parse( root.Data.ArrivalTime)
                        .AddDays(root.Data.OrderRoomPrices.IndexOf(this)) },
                    { "Price", this.OriginSellPrice },
                    { "Persons", root.Data.Persons },
                    { "Rooms", root.Data.Quantity },
                    { "Nights", 1 },
                    { "Status", root.Data.OrderStatusDisplay }
                };

            #endregion

            public string LivingDate { get; set; }
            public string RoomPrice { get; set; }
            public string CostPrice { get; set; }
            public string SellPrice { get; set; }
            public string GuidCurrency { get; set; }
            public string Currency { get; set; }
            public string Breakfast { get; set; }
            public string OriginCostPrice { get; set; }
            public string OriginSellPrice { get; set; }
            public string OriginRoomPrice { get; set; }
            public string IsSalePrice { get; set; }
            public string ShowRightsTrip { get; set; }
            public string RightBreakFirst { get; set; }
            public List<PromotionItems> PromotionItems { get; set; }
            public string ChildPrices { get; set; }
        }

        public class SubOrders
        {
            public string EbkOrderID { get; set; }
            public string Token { get; set; }
            public string OrderType { get; set; }
            public string HotelId { get; set; }
            public string OrderStatus { get; set; }
            public string CtripSourceType { get; set; }
            public string CtripSourceTypeDisplay { get; set; }
            public string OrderID { get; set; }
            public string FormID { get; set; }
            public string FormDate { get; set; }
            public string FormDateDisplay { get; set; }
            public string OriginalOrderID { get; set; }
            public string ConfirmName { get; set; }
            public string IsSelected { get; set; }
            public string IsUseless { get; set; }
            public string IsUnprocess { get; set; }
            public string IsTop { get; set; }
            public string IsBottom { get; set; }
            public string IsCreditOrder { get; set; }
            public string IsCreditOrderConfirmed { get; set; }
            public string GuaranteeType { get; set; }
            public string Remarks { get; set; }
            public string CtripRemark { get; set; }
            public string CommonRemark { get; set; }
            public string PreModifyFormStatus { get; set; }
            public string ConfirmRemarkForOther { get; set; }
            public string RoomName { get; set; }
            public string RoomEName { get; set; }
            public string Quantity { get; set; }
            public string IsHoldRoom { get; set; }
        }

        public class Historys
        {
            public string OrderID { get; set; }
            public List<SubOrders> SubOrders { get; set; }
        }

        public class ContactInfo
        {
            public string Tel { get; set; }
            public string TelCaptcha { get; set; }
        }

        public class ZTCPaymentList
        {
            public string Name { get; set; }
            public string NameDisplay { get; set; }
            public string CostGapPriceDisplay { get; set; }
            public string SellGapPriceDisplay { get; set; }
            public string Currency { get; set; }
            public string CostGapPrice { get; set; }
            public string CostDiscount { get; set; }
            public string SellDiscount { get; set; }
            public string SellGapPrice { get; set; }
            public string AmountText { get; set; }
            public string ShowSalePrice { get; set; }
            public string RuleName { get; set; }
            public string PromotionRuleNameDisplay { get; set; }
        }

        public class LadderDeductPolicy
        {
        }

        public class RightsInfo
        {
        }

        public class RightsVMInfo
        {
        }

        public class Data
        {
            #region Extra Properties

            public SODict UserInfo => new SODict()
            {
                { "FullName", this.FullName },
                { "Phone", this.ContactInfo is null ? null : this.ContactInfo.Tel },
                { "PhoneCaptcha", this.ContactInfo is null ? null : this.ContactInfo.TelCaptcha }
            };

            public SODict ResInfo => new SODict()
            {
                { "Type", this.Type },
                { "ResNumber", this.OrderID },
                { "Channel", "Ctrip" },
                { "Source", this.AllinanceName },
                { "Subtotal",  this.Subtotal },
                { "Commission", this.Commission },
                { "BookedDateTime", DateTime.Parse(this.Historys[0].SubOrders[0].FormDate) },
                { "ArrivalDate", DateTime.Parse(this.ArrivalTime) },
                { "DepartureDate", DateTime.Parse(this.Departure) },
                { "ArrivalTime", this.ArrivalEarlyAndLatestTime },
                { "Persons", this.Persons },
                { "Nights", this.Nights },
                { "Rooms", int.Parse(this.Quantity) },
                { "Comments", this.Confirmremarks },
                { "Status", this.OrderStatusDisplay }
            };
            
            public string FullName
            {
                get
                {
                    int index = this.ClientName.IndexOf("&nbsp;");
                    string name = index > -1 ? this.ClientName.Substring(0, index) : this.ClientName;
                    return name.Replace("/", " ").Replace(",", "/");
                }
            }

            public int Type
            {
                get
                {
                    switch (this.OrderType)
                    {
                        case "C":
                            return 1;
                        case "M":
                            return 2;
                        case "D":
                            return 3;
                        case "N":
                        default:
                            return 0;
                    }
                }
            }

            public float Subtotal
            {
                get
                {
                    float subtotal = 0;
                    this.OrderRoomPrices.ForEach(
                        price => subtotal += float.Parse(price.OriginSellPrice));
                    return subtotal;
                }
            }

            public float Commission
            {
                get
                {
                    float balance = 0;
                    this.OrderRoomPrices.ForEach(
                        price => balance += float.Parse(price.OriginCostPrice));
                    return this.Subtotal - balance;
                }
            }

            public int Persons
            {
                get
                {
                    Regex r = new Regex(@"(?<=<b>\s*)\d+(?=\s*</b>)");
                    return r.IsMatch(this.ClientName) ? 
                        int.Parse(r.Match(this.ClientName).Groups[0].Value) : 0;
                }
            }

            public int Nights
            {
                get
                {
                    Regex r = new Regex(@"(?<=<b>\s*)\d+(?=\s*</b>)");
                    return r.IsMatch(this.ArrivalAndDeparture) ?
                        int.Parse(r.Match(this.ArrivalAndDeparture).Groups[0].Value) : 0;
                }
            }

            public int Status
            {
                get
                {
                    switch (this.OrderStatusDisplay)
                    {
                        case "待处理":
                            return -1;
                        case "已拒单":
                            return 1;
                        case "已取消":
                            return 2;
                        case "已接单":
                        default:
                            return 0;
                    }
                }
            }

            #endregion

            public string OrderType { get; set; }
            public string OrderTypeDisplay { get; set; }
            public string SourceType { get; set; }
            public string SourceTypeDisplay { get; set; }
            public string OrderStatus { get; set; }
            public string OrderStatusDisplay { get; set; }
            public string OrderID { get; set; }
            public string OrderKey { get; set; }
            public string FormID { get; set; }
            public string FormDate { get; set; }
            public string IsHoldRoom { get; set; }
            public string IsCreditOrder { get; set; }
            public string IsGuaranteed { get; set; }
            public string CompanyName { get; set; }
            public string IsResend { get; set; }
            public string IsLRA { get; set; }
            public string IsFreeSale { get; set; }
            public string IsUrgent { get; set; }
            public string IsPP { get; set; }
            public string IsFG { get; set; }
            public string HotelName { get; set; }
            public string IsShowHotelName { get; set; }
            public string HotelID { get; set; }
            public string RoomName { get; set; }
            public string Quantity { get; set; }
            public string AmountDes { get; set; }
            public string PaymentInfo { get; set; }
            public string OrgPaymentInfo { get; set; }
            public string PriceMode { get; set; }
            public string ZTCPaymentInfo { get; set; }
            public string PaymentTerm { get; set; }
            public string ClientName { get; set; }
            public string GiftInfo { get; set; }
            public string NotifyType { get; set; }
            public string CommonRemark { get; set; }
            public string RemarkTip { get; set; }
            public string Remarks { get; set; }
            public string CtripRemarks { get; set; }
            public string Confirmremarks { get; set; }
            public string ConfirmName { get; set; }
            public string ConfirmStatus { get; set; }
            public string BookingNO { get; set; }
            public string HideBookingNO { get; set; }
            public string ArrivalTime { get; set; }
            public string Departure { get; set; }
            public string ArrivalAndDeparture { get; set; }
            public string ArrivalEarlyAndLatestTime { get; set; }
            public string ArrivalInfo { get; set; }
            public string ArrivalWay { get; set; }
            public string AddBreakFastHtml { get; set; }
            public string AddOptionalHtml { get; set; }
            public Invoice Invoice { get; set; }
            public List<OrderRoomPrices> OrderRoomPrices { get; set; }
            public string IsCanSendSms { get; set; }
            public string IsShowPCC { get; set; }
            public string IsShowVCC { get; set; }
            public string NeedConfirmCreditOrder { get; set; }
            public string IsVip { get; set; }
            public string VipInfoDescription { get; set; }
            public List<Historys> Historys { get; set; }
            public string HistoryCount { get; set; }
            public ContactInfo ContactInfo { get; set; }
            public string PromotionInfo { get; set; }
            public string Changes { get; set; }
            public string PreChanges { get; set; }
            public string ChangeSummary { get; set; }
            public string EnableConfirm { get; set; }
            public string ShowRejectCancelBtn { get; set; }
            public string HideRejectBtn { get; set; }
            public string OfficalSealPath { get; set; }
            public string SupplierName { get; set; }
            public string BookDescription { get; set; }
            public string GuaranteeType { get; set; }
            public string CtripOrHotelGuaranteeType { get; set; }
            public string GuaranteeTypeTips { get; set; }
            public string EncryptID { get; set; }
            public string Token { get; set; }
            public string ShowPrint { get; set; }
            public string ShowPrintFAX { get; set; }
            public string ShowLog { get; set; }
            public string IsBookingInvoice { get; set; }
            public string InvoiceKey { get; set; }
            public string InvoiceToken { get; set; }
            public string ConfirmOtherWayType { get; set; }
            public string IsNewVersion { get; set; }
            public string IsShowRemarks { get; set; }
            public string IsNeedArrivalEarlyPay { get; set; }
            public string IsLastOrderConfirm { get; set; }
            public string IsDomesticOrder { get; set; }
            public string TotalFee { get; set; }
            public string Amount { get; set; }
            public string TotalFeeCurrency { get; set; }
            public string AdditonalFeeCurrency { get; set; }
            public string IsOnlineRoomChooseOrder { get; set; }
            public string NeedConfirmOnlineRoomChooseOrder { get; set; }
            public string RoomNo { get; set; }
            public string IsRiskyOrder { get; set; }
            public string IsShowNoCancelPolicy { get; set; }
            public string IsShowLimitCancelPolicy { get; set; }
            public string CancelPolicyTitle { get; set; }
            public string CancelPolicyText { get; set; }
            public string IsHighRiskOrder { get; set; }
            public string BedRemarks { get; set; }
            public string RoomPriceText { get; set; }
            public string SpRemarks { get; set; }
            public string IsOpenVcc { get; set; }
            public string CityId { get; set; }
            public string Province { get; set; }
            public string FormDateOriginal { get; set; }
            public string IsAutoConfirmed { get; set; }
            public string CtripSourceTypeDisplay { get; set; }
            public string CtripSourceType { get; set; }
            public string PurchaseCodes { get; set; }
            public string IsShowPurchaseCodes { get; set; }
            public string PreModifyFormStatus { get; set; }
            public string ContractNO { get; set; }
            public List<ZTCPaymentList> ZTCPaymentList { get; set; }
            public string IsShowPromotionTip { get; set; }
            public string ShowGuidedPrice { get; set; }
            public string IsCON { get; set; }
            public string IsSalePrice { get; set; }
            public string IsShowClientTime { get; set; }
            public string ClientTime { get; set; }
            public string IsPCCToVCC { get; set; }
            public string RefOrderID { get; set; }
            public List<LadderDeductPolicy> LadderDeductPolicy { get; set; }
            public string LadderDeductPolicyText { get; set; }
            public string Xcsmz { get; set; }
            public string OriginalOrderid { get; set; }
            public List<RightsInfo> RightsInfo { get; set; }
            public List<RightsVMInfo> RightsVMInfo { get; set; }
            public string ShowRigthsInfo { get; set; }
            public string IsShowRightInfoCancelPolicy { get; set; }
            public string RightInfoCancelPolicy { get; set; }
            public string IsShowRightInfoCancelPolicyRemark { get; set; }
            public string IsShowLadderDeductPolicy { get; set; }
            public string AllinanceName { get; set; }
            public string IsHourRoom { get; set; }
            public string IsHotelOrderConfirmNotChoose { get; set; }
            public string IsFullyBookedOrder { get; set; }
            public string GroupOrderType { get; set; }
            public string BreakfirstRightList { get; set; }
            public string LimitCancelHtml { get; set; }
            public string ThOrderIdInfo { get; set; }
            public string ThOrderId { get; set; }
        }

        public class RootObject
        {
            public List<SODict> RoomInfos
            {
                get
                {
                    List<SODict> rooms = new List<SODict>();
                    this.Data.OrderRoomPrices.ForEach(price => 
                        rooms.Add(price.ToDictionary(this)));
                    return rooms;
                }
            }

            public string Rcode { get; set; }
            public string Msg { get; set; }
            public string Script { get; set; }
            public Data Data { get; set; }
            public string TotalPage { get; set; }
            public string TotalRecords { get; set; }
        }
    }
}
