using YuI.EControls;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MementoConnection;
using MMC = MementoConnection.MMConnection;
using BookingElf;

namespace YuI
{
    public partial class Page_Reservation
    {
        public string BuildRoomDetails(string staffName)
        {
            if (!(rlv_res.SelectedItem is BubbleBookingItem Res)) return null;
            int roomCount = 0;
            int rtIndex = 0;
            string[] d = _XmlReader.ReadValue("Email/Day").Split(',');
            string[] m = _XmlReader.ReadValue("Email/" + _XmlReader.ReadValue("Email/MonthType")).Split(',');
            StringBuilder sb = new StringBuilder();
            string[] cs = _XmlReader.ReadValue("Email/Count").Split(',');
            bool IsSingleOrder = true;
            roomCount = 0;
            string SQL = "SELECT DISTINCT Type,Persons,Price,Rooms FROM info_res_rooms WHERE ResNumber='" +
                    Res.ResNumber + "'";
            DataTable roomt = MMC.Select(SQL);
            foreach (DataRow dsrow in roomt.Rows)
            {
                SQL = "SELECT Min(ReservedDate) As ArrivalDate,Max(ReservedDate) As DepartureDate FROM info_res_rooms" +
                     " WHERE ResNumber='" + Res.ResNumber + "' And Type='" + dsrow[0].ToString() + "'";
                DataTable dateDT = MMC.Select(SQL);
                rtIndex = MMC.GetItem<int>("select RTID from info_room_type_matches where MatchChar='" + dsrow[0].ToString() + "'") ?? 0;
                roomCount = rtIndex > 3 ? 1 : (int)dsrow[1];
                sb.Append(new string[] { " and ", "" }[Convert.ToInt32(IsSingleOrder)]);
                DataRow dateRow = dateDT.Rows[0];
                DateTime arrivalDate = DateTime.Parse(dateRow[0] as string);
                DateTime departureDate = DateTime.Parse(dateRow[1] as string);
                sb.Append("from " + d[arrivalDate.Day - 1]);
                sb.Append(new string[] { " " + m[arrivalDate.Month - 1], "" }[Convert.ToInt32(arrivalDate.Month == departureDate.AddDays(1).Month)]);
                sb.Append(" - " + d[departureDate.AddDays(1).Day - 1] + " " + m[departureDate.AddDays(1).Month - 1] + " " + departureDate.Year + "(check out) for ");
                sb.Append(cs[new int[] { (int)dsrow[1] - 1, 0 }[Convert.ToInt32(rtIndex > 2)]] + " ");
                sb.Append(MMC.GetString("select " + new string[] { "PluralEmailCaption_en", "SingularEmailCaption_en" }[Convert.ToInt32(roomCount == 1)] +
                                           " from info_room_types where RTID=" + rtIndex).ToString());
                double price = (double)dsrow[2];
                if (Res.Channel == "HostelWorld")
                    price *= new int[] { 1, (int)dsrow[1] }[Convert.ToInt32(rtIndex > 2)];
                sb.Append(" at " + price + " CNY/Night" + new string[] { "", "/Person" }[Convert.ToInt32(rtIndex < 3)]);
                IsSingleOrder = false;
            }
            return _XmlReader.ReadValue("Email/EmailBody").Replace("RoomDetails", sb.ToString()).Replace("StaffName", staffName).Replace("CustomerName", _InitializedName(Res.FullName));
        }

        private string _InitializedName(string name)
        {
            (int index1, int index2) = (name.IndexOf(" "), name.IndexOf("/"));
            int indexResult = index1 > -1 ? 1 : 0 + index2 > -1 ? 1 : 0;
            switch (indexResult)
            {
                case 2:
                    name = name.Substring(0, Math.Min(index1, index2));
                    break;
                case 1:
                    name = name.Substring(0, Math.Max(index1, index2));
                    break;
                case 0:
                default:

                    break;
            }
            return name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();
        }

        public string GetHttpHtml(string HttpWebSite,string WebOrderNumber)
        {
            HttpWebRequest httpReq = WebRequest.Create(new Uri(_XmlReader.ReadValue(HttpWebSite + "/LoginSite"))) as HttpWebRequest;
            CookieContainer Cookies = new CookieContainer();
            httpReq.CookieContainer = new CookieContainer();
            httpReq.Proxy = null;
            httpReq.Referer = _XmlReader.ReadValue(HttpWebSite + "/RefererSite");
            httpReq.Method = "POST";
            httpReq.Accept = "text/html, application/xhtml+xml, */*";
            httpReq.ContentType = "application/x-www-form-urlencoded";
            httpReq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            httpReq.KeepAlive = true;
            httpReq.AllowAutoRedirect = false;
            httpReq.Credentials = CredentialCache.DefaultCredentials;
            httpReq.Method = "POST";
            byte[] PostByte = Encoding.GetEncoding("GB2312").GetBytes(_XmlReader.ReadValue(HttpWebSite + "/PostString"));
            httpReq.ContentLength = PostByte.Length;
            Stream PostStream = httpReq.GetRequestStream();
            PostStream.Write(PostByte, 0, PostByte.Length);
            PostStream.Close();
            Cookies = httpReq.CookieContainer;
            HttpWebResponse httpRes = (HttpWebResponse)(httpReq.GetResponse());
            foreach (Cookie ck in httpRes.Cookies)
            {
                Cookies.Add(ck);
                Console.WriteLine(ck.Name);
            }
            httpReq = WebRequest.Create(new Uri(_XmlReader.ReadValue(HttpWebSite + "/RevSite").Replace("OrderNumber", WebOrderNumber))) as HttpWebRequest;
            httpReq.CookieContainer = Cookies;
            httpReq.Method = "GET";
            Stream stream = (httpReq.GetResponse() as HttpWebResponse).GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string respHTML = reader.ReadToEnd();
            //Clipboard.SetText(respHTML);
            reader.Close();
            stream.Close();
            if (httpReq != null)
            {
                httpReq.GetResponse().Close();
                httpReq = null;
            }
            return respHTML;
        }
    }
}
