using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ELite;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using ELite.Reservation;
using System.Net;
using Microsoft.Scripting.Hosting;

namespace ELite.Reservation
{
    public enum ResGettingMethod { CLIPBOARD, HTTP }
    public enum RexItemType { BOOLEAN, INTEGER, FLOAT, DATETIME, STRING }

    public class ResGetter
    {
        private EXmlReader _XmlReader;
        private ELiteConnection _Conn;
        private Logger _Logger;
        private string _CrtText;
        private ELiteListBoxResItem _CrtLBResItem;
        private Booking _CrtBooking;

        #region MAIN

        /// <summary> 初始化订单获取器，需要路径参数，默认为类库工作路径，此路径用于
        /// 初始化获取器的xml配置文件路径 </summary>
        public ResGetter(ELiteConnection conn, string path = "")
        {
            _Conn = conn;
            if (path == null || path == "" || !Directory.Exists(path))
                path = Environment.CurrentDirectory;
            _XmlReader = new EXmlReader(path, "resgetter");
            _XmlReader.Open();
            _Logger = new Logger(path, "resgetter");
            _Logger.Open();
        }

        /// <summary> 对传入的文本信息进行分析，提取订单号及订单详情，成功则返回订单号，
        /// 失败时返回空值。 </summary>
        public ELiteListBoxResItem GetReservation(string text)
        {
            _CrtText = text;
            if (String.IsNullOrEmpty(_CrtText)) return null;
            GetListBoxResItem();
            if (_CrtLBResItem.ResNumber.Length == 0 || _CrtLBResItem.Channel.Length == 0) return null;
            //若标记为此Channel不可用，则返回空值。
            if (_XmlReader.ReadValue("Channels/" + _CrtLBResItem.Channel + "/Enabled") == "0") return null;
            //若标记为HTTP获取，需要自动登录获取订单页面文本，将替换被提取订单信息的文本
            ResGettingMethod method = (ResGettingMethod)(Convert.ToInt32(
                _XmlReader.ReadValue("Channels/" + _CrtLBResItem.Channel + "/Method")));
            if (method == ResGettingMethod.HTTP) _CrtText = GetHttpHtmlText();
            //从订单源文本中分析提取订单详情，成功返回订单，失败返回空值
            if (SaveResItem()) return _CrtLBResItem;
            _CrtBooking = null;
            _CrtText = null;
            return null;
        }

        #endregion
        
        #region STEPS

        /// <summary> 获取订单。步骤1：获取订单源和订单号的类的实例。 </summary>
        private void GetListBoxResItem()
        {
            Regex regex;
            _CrtLBResItem = ELiteListBoxResItem.Empty;
            //列出所有订单源，并尝试获取订单号
            foreach (XmlNode node in _XmlReader.ReadNodes("Channels"))
            {
                regex = new Regex(node.SelectSingleNode("Rex-Channel").InnerText);
                string channel = regex.Match(_CrtText).Groups[0].Value;
                if (String.IsNullOrEmpty(channel)) continue;
                //使用正则表达式匹配订单号
                regex = new Regex(node.SelectSingleNode("Rex-ResNumber").InnerText);
                string resNumber = regex.Match(_CrtText).Groups[0].Value;
                if (String.IsNullOrEmpty(resNumber)) continue;
                _CrtLBResItem.Channel = ELiteConnection.Channels.Find(c => c.Name == channel).Title_en_us;
                _CrtLBResItem.ResNumber = resNumber;
                _CrtLBResItem.FullName = ELiteConnection.DefaultUserName;
                break;
            }
        }

        /// <summary> 获取订单。步骤1.5：为Method为Http获取方式的订单源获取页面代码。 </summary>
        public string GetHttpHtmlText()
        {
            #region 登录

            string uriString = _XmlReader.ReadValue(
                "Channels/" + _CrtLBResItem.Channel + "/Net/Site-Login");
            HttpWebRequest httpReq = WebRequest.CreateHttp(uriString);
            httpReq.Method = "POST";
            httpReq.Accept = "text/html, application/xhtml+xml, */*";
            httpReq.ContentType = "application/x-www-form-urlencoded";
            httpReq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            httpReq.KeepAlive = true;
            httpReq.AllowAutoRedirect = false;
            httpReq.Credentials = CredentialCache.DefaultCredentials;
            byte[] postByte = Encoding.GetEncoding("GB2312").GetBytes(_XmlReader.ReadValue(
                "Channels/" + _CrtLBResItem.Channel + "/Net/PostString-Login"));
            httpReq.ContentLength = postByte.Length;
            Stream postStream = httpReq.GetRequestStream();
            postStream.Write(postByte, 0, postByte.Length);
            postStream.Close();
            CookieContainer Cookies = httpReq.CookieContainer;
            HttpWebResponse httpRes = httpReq.GetResponse() as HttpWebResponse;

            #endregion
            foreach (Cookie ck in httpRes.Cookies)
            {
                Cookies.Add(ck);
            }
            #region 获取订单页面

            string resUriString = _XmlReader.ReadValue(
                "Channels/" + _CrtLBResItem.Channel + "/Net/Site-Res");
            httpReq = WebRequest.CreateHttp(resUriString);
            httpReq.CookieContainer = Cookies;
            httpReq.Method = "GET";
            HttpWebResponse httpResp = httpReq.GetResponse() as HttpWebResponse;
            StreamReader sr = new StreamReader(httpResp.GetResponseStream(),
                Encoding.GetEncoding("GB2312"));
            string respHtml = sr.ReadToEnd();
            sr.Close();

            #endregion
            return respHtml;
        }

        public static string GetHWRes(string resNumber)
        {
            /*var options = new Dictionary<string, object>();
            options["Frames"] = true;
            options["FullFrames"] = true;
            ScriptEngine pyEngine = Python.CreateEngine(options);//创建Python解释器对象
            Console.WriteLine(pyEngine.LanguageVersion.ToString());
            dynamic py = pyEngine.ExecuteFile("HostelWorld.py");//读取脚本文件
            py.Login();
            return py.Get(resNumber);//调用脚本文件中对应的函数*/
            return "";
        }

        /// <summary> 获取订单。步骤2：使用正则表达式从订单源文本中匹配订单详情，
        /// 并保存到数据库。 </summary>
        private bool SaveResItem()
        {
            _CrtBooking = new Booking();
            RegexResUserItems();
            RegexResItems();
            RegexResRoomItems();
            try
            {
                string resNumber = _CrtBooking.ResItem.ResNumber;
            }
            catch
            {
                _Logger.Log("订单号为空！", "");
                return false;
            }
            _Conn.InsertRes(_CrtBooking);
            return true;
        }

        #endregion

        #region REGEX

        /// <summary> 使用正则表达式从源文本中匹配订单详情项。 </summary>
        private void RegexResItems()
        {
            Dictionary<string, object> dbResItems = RegexItems(_XmlReader.ReadNode(
                "Channels/" + _CrtLBResItem.Channel + "/Rex-Res"), _CrtText);
            dbResItems.Add("uid", _CrtBooking.ResUserItem.UID);
            dbResItems.Add("ResNumber", _CrtLBResItem.ResNumber);
            dbResItems.Add("Channel", ELiteConnection.Channels.Find(
                c => c.Name == _CrtLBResItem.Channel).ID);
            int maxLid = 0;
            try
            {
                maxLid = Convert.ToInt32(_Conn.ReadValue(
                    "select max(sid) from log_operation"));
            }
            catch
            {

            }
            dbResItems.Add("lid", maxLid + 1);
            _CrtBooking.ResItem = new DBResItem(dbResItems);
        }

        /// <summary> 使用正则表达式从源文本中匹配用户详情项。 </summary>
        private void RegexResUserItems()
        {
            Dictionary<string, object> dbResUserItems = RegexItems(_XmlReader.ReadNode(
                "Channels/" + _CrtLBResItem.Channel + "/Rex-User"), _CrtText);
            int maxUid = 0;
            try
            {
                maxUid = Convert.ToInt32(_Conn.ReadValue("select max(uid) from info_user"));
            }
            catch
            {

            }
            dbResUserItems.Add("uid", maxUid + 1);
            #region 修正姓名
            
            if (dbResUserItems.ContainsKey("GuestName"))
            {
                dbResUserItems.Remove("GivenName");
                dbResUserItems.Remove("MiddleName");
                dbResUserItems.Remove("Surname");
                string name = (string)dbResUserItems["GuestName"];
                int emptyCount = name.Length - name.Replace(" ", "").Length;
                int emptyIndex = name.IndexOf(" ");
                int lastEmptyIndex = name.LastIndexOf(" ");
                switch (emptyCount)
                {
                    case 0:
                        dbResUserItems.Add("GivenName", name);
                        break;
                    case 1:
                        dbResUserItems.Add("GivenName", name.Substring(0, emptyIndex));
                        dbResUserItems.Add("Surname", name.Substring(emptyIndex + 1));
                        break;
                    default:
                        dbResUserItems.Add("GivenName", name.Substring(0, emptyIndex));
                        dbResUserItems.Add("MiddleName", name.Substring(emptyIndex + 1, 
                            lastEmptyIndex - emptyIndex - 1));
                        dbResUserItems.Add("Surname", name.Substring(lastEmptyIndex + 1));
                        break;
                }
                dbResUserItems.Remove("GuestName");
            }

            #endregion
            _CrtBooking.ResUserItem = new DBResUserItem(dbResUserItems);
        }

        /// <summary> 使用正则表达式从源文本中获取房间详情字典的列表。 </summary>
        private bool RegexResRoomItems()
        {
            string roomRegexText = _XmlReader.ReadValue(
                "Channels/" + _CrtLBResItem.Channel + "/Rex-Rooms");//匹配房间的正则表达式文本
            if (string.IsNullOrEmpty(roomRegexText)) return false;
            Regex regex = new Regex(roomRegexText);
            MatchCollection matches = regex.Matches(_CrtText);//预订的房间通常不止一个
            if (matches.Count < 1) return false;
            List<Dictionary<string, object>> roomDictList = new List<Dictionary<string, object>>();
            foreach (Match roomMatch in matches)
            {
                string roomText = roomMatch.Groups[0].Value;
                if (String.IsNullOrEmpty(roomText)) continue;
                Dictionary<string, object> resRoomItemDict = RegexItems(_XmlReader.ReadNode(
                    "Channels/" + _CrtLBResItem.Channel + "/Rex-Room"),
                    _CrtText, roomMatch.Groups[0].Value);
                if (resRoomItemDict == null || resRoomItemDict.Count < 1) continue;
                resRoomItemDict.Add("ResNumber", _CrtLBResItem.ResNumber);
                roomDictList.Add(resRoomItemDict);
            }
            if (roomDictList.Count < 1) return false;
            List<DBResRoomItem> roomItemList = new List<DBResRoomItem>();
            roomDictList.ForEach(dict => roomItemList.Add(new DBResRoomItem(dict)));
            _CrtBooking.ResRoomItemList = roomItemList;
            return true;
        }

        /// <summary> 通用方法。使用正则表达式从源文本中获取详情字典。 </summary>
        private Dictionary<string, object> RegexItems(XmlNode rexRootNode, 
            string text, string roomText = "")
        {
            XmlNodeList nodeList = rexRootNode.ChildNodes;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (XmlNode regexNode in rexRootNode.ChildNodes)
            {
                string nodeName = regexNode.Name;
                string regexText = regexNode.InnerText;
                if (regexText == null || regexText.Length < 4) continue;//确保正则表达式不为空
                if (regexText.StartsWith("Res."))
                    dict.Add(nodeName, _CrtBooking.ResItem.GetValue(regexText.Substring(4)));
                else if(regexText.StartsWith("User."))
                    dict.Add(nodeName, _CrtBooking.ResUserItem.GetValue(regexText.Substring(5)));
                else
                {
                    RexItemType type = (RexItemType)(Convert.ToInt32(regexText.Substring(0, 1)));
                    #region 匹配值为空则不计入结果

                    string isRegexRoomText = regexText.Substring(1, 1);
                    regexText = regexText.Substring(3);
                    Regex regex = new Regex(regexText);
                    string resultStr = string.Empty;
                    if (isRegexRoomText != "0")
                        resultStr = regex.Match(roomText).Groups[0].Value;
                    else
                        resultStr = regex.Match(text).Groups[0].Value;
                    if (string.IsNullOrEmpty(resultStr)) continue;

                    #endregion
                    #region 将结果转为预设的格式并计入字典，默认为字符串格式

                    object resultObj = resultStr;
                    switch (type)
                    {
                        case RexItemType.BOOLEAN://0
                            resultObj = Convert.ToBoolean(resultStr);
                            break;
                        case RexItemType.INTEGER://1
                            resultObj = Convert.ToInt32(resultStr);
                            break;
                        case RexItemType.FLOAT://2
                            resultObj = Convert.ToSingle(resultStr);
                            break;
                        case RexItemType.DATETIME://3
                            resultObj = StringToDateTime(resultStr);
                            break;
                    }
                    dict.Add(nodeName, resultObj);

                    #endregion
                }
            }
            if (dict.Count < 1) dict = null;
            return dict;
        }

        private DateTime StringToDateTime(string text)
        {
            DateTime result = DateTime.Parse("1997/03/03");
            #region 替换字符，初始化日期字符串格式

            string replaceItemsString = _XmlReader.
                ReadValue("Channels/" + _CrtLBResItem.Channel + "/DateTimeReplaceItems");
            if (String.IsNullOrEmpty(replaceItemsString)) return result;
            foreach (string repText in replaceItemsString.Split(Convert.ToChar("|")))
            {
                if (repText == null) continue;
                int index = repText.IndexOf(",");
                if (index < 0 || repText.Length < index + 2) continue;
                if (repText.Substring(index + 1, 1) == ",") index += 1;
                //以上代码确保英文逗号可替换和可被替换
                text = text.Replace(repText.Substring(0, index), repText.Substring(index + 1));
            }
            text = text.Replace("null", "");

            #endregion
            #region 匹配相应日期字符串格式，转换为DateTime格式

            string dtFormatString = _XmlReader.
                ReadValue("Channels/" + _CrtLBResItem.Channel + "/DateTimeFormatItems");
            if (String.IsNullOrEmpty(dtFormatString)) return result;
            try
            {
                result = DateTime.ParseExact(text, dtFormatString.Split(Convert.ToChar("|")),
                    System.Globalization.DateTimeFormatInfo.InvariantInfo,
                    System.Globalization.DateTimeStyles.None);
            }
            catch (Exception ex)
            {
                _Logger.Log("日期转换失败", text + "\r\n" + dtFormatString + "\r\n" + ex.Message);
            }

            #endregion
            return result;
        }
        
        #endregion
        
    }
}
