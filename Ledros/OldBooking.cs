using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using MMC = MementoConnection.MMConnection;

namespace Ledros
{
    public class OldBooking
    {
        #region GETRES

        public static EXmlReader XmlReader;

        ConceivingBookingItem _AliveBubble;

        public void SniffRes(string text)
        {
            foreach (string channel in XmlReader.ReadValue("Main/Clip-WebSite").Split(','))
            {
                if (text.IndexOf(XmlReader.ReadValue(channel + "/ClipboardKeyword")) < 0) continue;
                _AliveBubble = new ConceivingBookingItem() { Channel = channel };
                if (XmlReader.ReadValue(channel + "/IsNet") != "0")
                {
                    _AliveBubble.FullName = GetRegexValue(XmlReader.ReadNode(channel + "/Reg-ResBalloon/FullName"), text);
                    _AliveBubble.ResNumber = GetRegexValue(XmlReader.ReadNode(channel + "/Reg-ResBalloon/ResNumber"), text);
                    text = GetHttpHtml("HostelWorld", _AliveBubble.ResNumber);
                }
                GetRes(channel, text);
                Clipboard.SetText(string.Empty);
                break;
            }
        }

        public void GetRes(string channel, string htmlText, string resNumber = null)
        {
            _AliveBubble = new ConceivingBookingItem() { Channel = channel, ResNumber = resNumber };
            _AliveBubble.UID = GetRes_UserItems(htmlText);
            if (_AliveBubble.UID == -1) { _AliveBubble = null; return; }
            int resResult = GetRes_ResItems(htmlText);
            switch (resResult)
            {
                case 1:
                    MMC.DeleteResByResNumber(_AliveBubble.ResNumber);
                    break;
                case 2:
                    Dispatcher.Invoke(new Action(() => FindRes(_AliveBubble.ResNumber)));
                    break;
                default:
                    if (GetRes_ResRoomItems(htmlText))
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            FindRes(_AliveBubble.ResNumber);
                            rlv_res.SelectedIndex = 0;
                        }));
                    }
                    else
                    {
                        MMC.DeleteResByResNumber(_AliveBubble.ResNumber);
                    }
                    break;
            }
            _AliveBubble = null;
        }

        public string GetHWRes()
        {
            string orderNumber = string.Empty;
            if (String.IsNullOrEmpty(orderNumber)) orderNumber = Interaction.InputBox(
                 "请输入订单号：", "订单号", "").Replace("89968-", "").Replace(" ", "");
            if (string.IsNullOrEmpty(orderNumber)) return null;
            if (!IsNumeric(orderNumber))
            {
                _ErrorQueue.Enqueue("错误的订单号：" + orderNumber + "！");
                return;
            }
            if (MMC.IsResExisted(orderNumber))
            {
                rlv_res.SelectedIndex = _Bubbles.IndexOfResNumber(orderNumber);
                return;
            }
            GetRes("HostelWorld", GetHttpHtml("HostelWorld", orderNumber), orderNumber);
        }

        private long GetRes_UserItems(string text)
        {
            Dictionary<string, object> userItemDict = new Dictionary<string, object>();
            foreach (XmlNode node in XmlReader.ReadNodes(_AliveBubble.Channel + "/Reg-User"))
            {
                userItemDict.Add(node.Name, GetRegexValue(node, text, _AliveBubble.Channel));
            }
            if (userItemDict.ContainsKey("FullName"))
            {
                _AliveBubble.FullName = userItemDict["FullName"] as string;
            }
            else
            {
                if (String.IsNullOrEmpty(_AliveBubble.FullName) || _AliveBubble.FullName == "六娃")
                {
                    string fullName = CombineFullName(userItemDict);
                    _AliveBubble.FullName = fullName.Length > 0 ? fullName : "六娃";
                }
                userItemDict.Add("FullName", _AliveBubble.FullName);
            }
            return MMC.InsertUser(userItemDict);
        }

        private string CombineFullName(Dictionary<string, object> itemDict)
        {
            string result = string.Empty;
            foreach (string nameKey in new string[] { "Surname", "MiddleName", "GivenName" })
            {
                if (itemDict.ContainsKey(nameKey)) result += " " + itemDict[nameKey];
            }
            return result.Length > 0 ? result.Substring(1) : result;
        }

        /// <summary> 返回值：0-新订单保存成功；1-新订单保存失败；2-已存在订单 </summary>
        private int GetRes_ResItems(string text)
        {
            Dictionary<string, object> resItemDict = new Dictionary<string, object>();
            XmlReader.ReadNodes(_AliveBubble.Channel + "/Reg-Res").ForEach(node =>
            {
                resItemDict.Add(node.Name, GetRegexValue(node, text, _AliveBubble.Channel));
            });
            if (!resItemDict.ContainsKey("ResNumber"))
                resItemDict.Add("ResNumber", _AliveBubble.ResNumber);
            else
                _AliveBubble.ResNumber = resItemDict["ResNumber"] as string;
            if (MMC.IsResExisted(_AliveBubble.ResNumber)) return 2;
            resItemDict.Add("uid", _AliveBubble.UID.ToString());
            resItemDict.Add("Channel", _AliveBubble.Channel);
            return MMC.InsertRes(resItemDict) ? 0 : 1;
        }

        private bool GetRes_ResRoomItems(string text)
        {
            //所有匹配的房间字符串
            foreach (Match roomMatch in Regex.Matches(text, XmlReader.ReadValue(_AliveBubble.Channel + "/Reg-Rooms")))
            {
                Dictionary<string, object> resRoomItemDict = new Dictionary<string, object>
                {
                    { "ResNumber", _AliveBubble.ResNumber },
                    { "uid", _AliveBubble.UID },
                };
                List<XmlNode> nodeList = XmlReader.ReadNodes(_AliveBubble.Channel + "/Reg-Room");
                nodeList.FindAll(node => node.Name != "Reg-Repeats" && node.Name != "Reg-Repeat").ForEach(node =>
                {
                    resRoomItemDict.Add(node.Name, GetRegexValue(node, roomMatch.Value, _AliveBubble.Channel));
                });
                if (resRoomItemDict.ContainsKey("FullName"))
                {
                    if (resRoomItemDict["uid"] == null)
                        resRoomItemDict["uid"] =
                                MMC.GetUIDByFullName(resRoomItemDict["FullName"] as string);
                    resRoomItemDict.Remove("FullName");
                }
                nodeList = XmlReader.ReadNodes(_AliveBubble.Channel + "/Reg-Room/Reg-Repeat");
                if (nodeList.Count > 0)//同一房间不同日期、价格等
                {
                    nodeList.ForEach(node => resRoomItemDict.Add(node.Name, ""));
                    if (resRoomItemDict.ContainsKey("ReservedDate"))
                    {
                        foreach (Match repeatMatch in Regex.Matches(roomMatch.Value,
                            XmlReader.ReadValue(_AliveBubble.Channel + "/Reg-Room/Reg-Repeats")))
                        {
                            nodeList.ForEach(node =>
                                resRoomItemDict[node.Name] = GetRegexValue(node, repeatMatch.Value, _AliveBubble.Channel));
                            if (!MMC.InsertResRoom(resRoomItemDict)) return false;
                        }
                    }
                    else if (resRoomItemDict.ContainsKey("ArrivalDate") &&
                        resRoomItemDict.ContainsKey("DepartureDate"))
                    {
                        Console.WriteLine(resRoomItemDict["ArrivalDate"].GetType());
                        DateTime arrivalDate = DateTime.Parse(resRoomItemDict["ArrivalDate"] as string);
                        DateTime departureDate = DateTime.Parse(resRoomItemDict["DepartureDate"] as string);
                        MatchCollection repeatResults = Regex.Matches(roomMatch.Value,
                            XmlReader.ReadValue(_AliveBubble.Channel + "/Reg-Room/Reg-Repeats"));
                        if ((departureDate - arrivalDate).Days != repeatResults.Count)
                        {
                            return false;
                        }
                        else
                        {
                            resRoomItemDict.Remove("ArrivalDate");
                            resRoomItemDict.Remove("DepartureDate");
                            resRoomItemDict.Add("ReservedDate", "");
                            for (int i = 0; i < repeatResults.Count; i++)
                            {
                                resRoomItemDict["ReservedDate"] = arrivalDate.AddDays(i).ToString("s");
                                nodeList.ForEach(node =>
                                    resRoomItemDict[node.Name] = GetRegexValue(node, repeatResults[i].Value, _AliveBubble.Channel));
                                if (!MMC.InsertResRoom(resRoomItemDict)) return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (!MMC.InsertResRoom(resRoomItemDict)) return false;
                }
            }
            return true;
        }

        private string GetRegexValue(XmlNode node, string text, string channel = "")
        {
            string innerText = node.InnerText;
            string value = Regex.Match(text, innerText.Substring(4)).Value;
            if (innerText.StartsWith("Dat"))
            {
                if (!String.IsNullOrEmpty(channel) && XmlReader.ReadNode(channel) != null)
                    value = ConvertDateFromRegexValue(channel, value);
            }
            else if (innerText.StartsWith("Num"))
            {
                value = value.Replace("，", "").Replace(",", "");
            }
            return value;
        }

        private string ConvertDateFromRegexValue(string channel, string str)
        {
            str = str.Replace(",", "");
            foreach (string rStr in XmlReader.ReadValue(channel + "/DateReplaceText").Split('|'))
            {
                if (!string.IsNullOrEmpty(rStr))
                {
                    int index = rStr.StartsWith(",") ? rStr.LastIndexOf(",") : rStr.IndexOf(",");
                    str = str.Replace(rStr.Substring(0, index), rStr.Substring(index + 1).Replace("null", "")).Trim();
                }
            }
            str = str.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            return String.IsNullOrEmpty(str) ? str : DateTime.ParseExact(str,
                XmlReader.ReadValue(channel + "/DateFormatText").Split('|'),
                System.Globalization.DateTimeFormatInfo.InvariantInfo,
                System.Globalization.DateTimeStyles.None).ToString("s");
        }
        
        public string GetHttpHtml(string HttpWebSite, string WebOrderNumber)
        {
            HttpWebRequest httpReq = WebRequest.Create(new Uri(XmlReader.ReadValue(HttpWebSite + "/LoginSite"))) as HttpWebRequest;
            CookieContainer Cookies = new CookieContainer();
            httpReq.CookieContainer = new CookieContainer();
            httpReq.Proxy = null;
            httpReq.Referer = XmlReader.ReadValue(HttpWebSite + "/RefererSite");
            httpReq.Method = "POST";
            httpReq.Accept = "text/html, application/xhtml+xml, */*";
            httpReq.ContentType = "application/x-www-form-urlencoded";
            httpReq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            httpReq.KeepAlive = true;
            httpReq.AllowAutoRedirect = false;
            httpReq.Credentials = CredentialCache.DefaultCredentials;
            httpReq.Method = "POST";
            byte[] PostByte = Encoding.GetEncoding("GB2312").GetBytes(XmlReader.ReadValue(HttpWebSite + "/PostString"));
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
                Console.WriteLine(ck.Domain);
            }
            httpReq = WebRequest.Create(new Uri(XmlReader.ReadValue(HttpWebSite + "/RevSite").Replace("OrderNumber", WebOrderNumber))) as HttpWebRequest;
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

        #endregion
    }
}
