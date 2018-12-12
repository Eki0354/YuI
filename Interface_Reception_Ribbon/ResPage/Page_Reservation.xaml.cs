using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Data;
using System.Net;
using System.IO;
//using System.Windows.Forms;
using ELite;
using System.Xml;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Windows.Controls.Ribbon;
using System.Windows;
using System.Threading;
using System.ComponentModel;
using System.Text;
using ResHelper;
using System.Windows.Threading;
using EControlsLibrary;
using System.Windows.Data;
using ELite.Reservation;

namespace Interface_Reception_Ribbon
{
    public partial class Page_Reservation : Page
    {
        #region PROPERTY

        string _ResConfigFullPath => MainWindow.AppDataPath + "\\ResConfig.xml";
        ResXmlReader _XmlReader;
        FileSystemWatcher _HtmlFileSystemWatcher;
        DispatcherTimer _ErrorDispatchTimer;
        Queue<string> _ErrorQueue = new Queue<string>();
        public ELiteConnection _Conn = MainWindow.Conn;
        ResHelper.ResHelper _ResHelper;
        public string CommentString => tb_order_content.Text;
        public string CellString
        {
            get
            {
                ELiteListBoxResItem res = rlv_res.SelectedResItem;
                if (res is null) return "未选中订单！";
                return _XmlReader.ReadValue(res.Channel + "/AbTitle") + "\r\n" + res.FullName;
            }
        }
        public string EmailThemeTemplet
        {
            get
            {
                string theme = _XmlReader.ReadValue("Email/EmailTheme");
                if (rlv_res.SelectedResItem == null ||
                    rlv_res.SelectedResItem.Channel != "TrafficYouth") return theme;
                theme = _XmlReader.ReadValue("Email/EmailTheme-TrafficYouth");
                return theme;
            }
        }
        public string EmailAddress
        {
            get
            {
                string content = tb_order_content.Text;
                if (string.IsNullOrEmpty(content)) return string.Empty;
                int startIndex = content.IndexOf("Email");
                if (startIndex < 0) return string.Empty;
                int endIndex = content.IndexOf("\r\n", startIndex);
                return content.Substring(startIndex + 7, endIndex - startIndex - 7);
            }
        }
        public ELiteListBoxResItem ActiveResItem => rlv_res.SelectedResItem;
        DateTime _UnknownDate => DateTime.Parse(_XmlReader.ReadValue("Main/UnknownDate"));

        #endregion

        #region Itinialize

        public Page_Reservation()
        {
            InitializeComponent();
            if (File.Exists(_ResConfigFullPath))
            {
                _XmlReader = new ResXmlReader(_ResConfigFullPath);
                _XmlReader.Open();
            }
            else
            {
                EMsgBox.ShowMessage("配置文件不存在！\r\n请检查是否已被删除。", "提示");
                Environment.Exit(0);
            }
            if (_XmlReader.ReadValue("Main/Enable") == "0") GetNPC();
            InitializeControls();
        }

        private void Page_Initialized(object sender, EventArgs e)
        {
            rlv_res.InitializeFrom(_Conn, lb_order_SelectionChanged);
            _ResHelper = new ResHelper.ResHelper();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeClipboardViewer();
        }

        /// <summary> 全称为'Get Nice Person Card'，当配置文件中Enabled值为0时可触发，使程序随机出错。 </summary>
        private void GetNPC()
        {
            DateTime dt = DateTime.Now;
            if (dt.Date.Month != 2 && (dt.Day != 14 || dt.Day != 15))
            {
                EMsgBox.ShowMessage(_XmlReader.ReadValue("Main/ErrorText"), "提示");
                Environment.Exit(0);
            }
        }

        #endregion

        #region Controls & EventHandles

        private void InitializeControls()
        {
            string htmlPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Html";
            if (!Directory.Exists(htmlPath)) Directory.CreateDirectory(htmlPath);
            _HtmlFileSystemWatcher = new FileSystemWatcher()
            {
                Path = htmlPath,
                Filter = "*.ht*",
                EnableRaisingEvents = true
            };
            _HtmlFileSystemWatcher.Changed += _HtmlFileSystemWatcher_Changed;
            _ErrorDispatchTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            _ErrorDispatchTimer.Tick += PopupError;
            _ErrorDispatchTimer.Start();
        }
        
        public void _HtmlFileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            FileStream fs = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            string htmlString = new StreamReader(fs).ReadToEnd();
            foreach (string htmlName in _XmlReader.ReadValue("Main/Keyword-WebSite").Split(','))
            {
                if (htmlString.IndexOf(htmlName) > -1)
                {
                    int index = 0;
                    if (htmlName == "Chengdu Traffic Youth Hostel")
                    {
                        index = 1;
                    }
                    else
                    {
                        index = 0;
                    }
                    GetRes(new string[] { "Booking", "TrafficYouth" }[index], htmlString);
                    break;
                }
            }
        }
        
        private void PopupError(object sender, EventArgs e)
        {
            while (_ErrorQueue.Count > 0)
            {
                tb_order_error.AppendText(_ErrorQueue.Dequeue() + "\r\n");
            }
        }

        #endregion

        #region GETRES

        public void SniffRes()
        {
            if (!Clipboard.ContainsText()) return;
            string text = string.Empty;
            try
            {
                text = Clipboard.GetText();
            }
            catch
            {
                return;
            }
            if (string.IsNullOrEmpty(text)) return;
            foreach (string channel in _XmlReader.ReadValue("Main/Clip-WebSite").Split(','))
            {
                if (text.IndexOf(_XmlReader.ReadValue(channel + "/ClipboardKeyword")) < 0) continue;
                ELiteListBoxResItem res = ELiteListBoxResItem.Empty;
                res.Channel = channel;
                if (_XmlReader.ReadValue(channel + "/IsNet") != "0")
                {
                    res.ResNumber = GetRegexValue(_XmlReader.ReadNode(channel + "/Reg-ResBalloon/ResNumber"), text);
                    res.FullName = GetRegexValue(_XmlReader.ReadNode(channel + "/Reg-ResBalloon/FullName"), text);
                    text = _ResHelper.GetHttpHtml("HostelWorld", res.ResNumber, _XmlReader);
                }
                GetRes(channel, text, res);
                Clipboard.SetText(string.Empty);
                break;
            }
        }

        public void GetRes(string channel, string htmlText, ELiteListBoxResItem res = null)
        {
            ELiteListBoxResItem newRes = res;
            if (newRes == null)
            {
                newRes = ELiteListBoxResItem.Empty;
                newRes.Channel = channel;
            }
            _ErrorQueue.Enqueue("代码获取完毕！正在读取详情 . . ." + "\r\n");
            int uid = GetRes_UserItems(htmlText, newRes);
            int resResult = GetRes_ResItems(htmlText, newRes, uid);
            if (resResult == 1)
            {
                _Conn.InvalidResByNumber(newRes.ResNumber);
                _ErrorQueue.Enqueue("获取订单信息出错！请检查错误日志 . . .");
                return;
            }
            else if (resResult == 2)
            {
                rlv_res.Find(newRes.ResNumber);
                return;
            }
            else
            {
                newRes.InitializeID(_Conn);
            }
            if (GetRes_ResRoomItems(htmlText, newRes, uid))
            {
                rlv_res.Add(newRes);
                //AddResBalloon(newRes);
                _ErrorQueue.Enqueue("获取完毕！已加入左侧订单列表 . . .");
            }
            else
            {
                _Conn.InvalidResByNumber(newRes.ResNumber);
                _ErrorQueue.Enqueue("获取房间出错！请检查错误日志 . . .");
            }
        }

        public void GetHWRes(ELiteListBoxResItem res = null)
        {
            string orderNumber = string.Empty;
            if (res != null)
                orderNumber = res.ResNumber;
            if (String.IsNullOrEmpty(orderNumber)) orderNumber = Interaction.InputBox(
                 "请输入订单号：", "订单号", "").Replace("89968-", "").Replace(" ", "");
            if (string.IsNullOrEmpty(orderNumber)) return;
            if (!IsNumeric(orderNumber))
            {
                _ErrorQueue.Enqueue("错误的订单号：" + orderNumber + "！");
                return;
            }
            if (_Conn.ExistValidRes(orderNumber))
            {
                rlv_res.Find(orderNumber);
                return;
            }
            GetRes("HostelWorld", _ResHelper.GetHttpHtml("HostelWorld", orderNumber, _XmlReader), res);
            _ErrorQueue.Enqueue("已成功获取HW订单详情！");
        }

        private int GetRes_UserItems(string text, ELiteListBoxResItem resItem)
        {
            Dictionary<string, string> userItemDict = new Dictionary<string, string>();
            foreach (XmlNode node in _XmlReader.ReadNodes(resItem.Channel + "/Reg-User"))
            {
                userItemDict.Add(node.Name, GetRegexValue(node, text, resItem.Channel));
            }
            if (userItemDict.ContainsKey("FullName"))
            {
                resItem.FullName = userItemDict["FullName"];
            }
            else
            {
                if (String.IsNullOrEmpty(resItem.FullName) || resItem.FullName == ELiteConnection.DefaultUserName)
                {
                    string fullName = CombineFullName(userItemDict);
                    resItem.FullName = fullName.Length > 0 ? fullName : ELiteConnection.DefaultUserName;
                }
                userItemDict.Add("FullName", resItem.FullName);
            }
            return _Conn.InsertUser(userItemDict);
        }

        private string CombineFullName(Dictionary<string, string> itemDict)
        {
            string result = string.Empty;
            foreach(string nameKey in new string[] { "Surname", "MiddleName", "GivenName" })
            {
                if (itemDict.ContainsKey(nameKey)) result += " " + itemDict[nameKey];
            }
            return result.Length > 0 ? result.Substring(1) : result;
        }

        /// <summary> 返回值：0-新订单保存成功；1-新订单保存失败；2-已存在订单 </summary>
        private int GetRes_ResItems(string text, ELiteListBoxResItem resItem, int uid)
        {
            Dictionary<string, string> resItemDict = new Dictionary<string, string>();
            _XmlReader.ReadNodes(resItem.Channel + "/Reg-Res").ForEach(node =>
            {
                resItemDict.Add(node.Name, GetRegexValue(node, text, resItem.Channel));
            });
            if (!resItemDict.ContainsKey("ResNumber"))
                resItemDict.Add("ResNumber", resItem.ResNumber);
            else
                resItem.ResNumber = resItemDict["ResNumber"];
            if (_Conn.ExistValidRes(resItem.ResNumber)) return 2;
            resItemDict.Add("uid", uid.ToString());
            resItemDict.Add("Channel", resItem.Channel);
            return _Conn.InsertRes(resItemDict) ? 0 : 1;
        }

        private bool GetRes_ResRoomItems(string text, ELiteListBoxResItem resItem, int uid)
        {
            //所有匹配的房间字符串
            foreach (Match roomMatch in Regex.Matches(text, _XmlReader.ReadValue(resItem.Channel + "/Reg-Rooms")))
            {
                Dictionary<string, string> resRoomItemDict = new Dictionary<string, string>
                {
                    //{ "ResNumber", resItem.ResNumber },
                    { "uid", uid.ToString() },
                    { "ResID", resItem.ID.ToString() }
                };
                List<XmlNode> nodeList = _XmlReader.ReadNodes(resItem.Channel + "/Reg-Room");
                nodeList.FindAll(node => node.Name != "Reg-Repeats" && node.Name != "Reg-Repeat").ForEach(node =>
                      {
                          resRoomItemDict.Add(node.Name, GetRegexValue(node, roomMatch.Value, resItem.Channel));
                      });
                if (resRoomItemDict.ContainsKey("FullName"))
                {
                    resRoomItemDict["uid"] = _Conn.GetUIDByFullName(resRoomItemDict["FullName"]).ToString();
                    resRoomItemDict.Remove("FullName");
                }
                nodeList = _XmlReader.ReadNodes(resItem.Channel + "/Reg-Room/Reg-Repeat");
                if (nodeList.Count > 0)//同一房间不同日期、价格等
                {
                    nodeList.ForEach(node => resRoomItemDict.Add(node.Name, ""));
                    if (resRoomItemDict.ContainsKey("ReservedDate"))
                    {
                        foreach (Match repeatMatch in Regex.Matches(roomMatch.Value,
                            _XmlReader.ReadValue(resItem.Channel + "/Reg-Room/Reg-Repeats")))
                        {
                            nodeList.ForEach(node =>
                                resRoomItemDict[node.Name] = GetRegexValue(node, repeatMatch.Value, resItem.Channel));
                            if (!_Conn.InsertResRoom(resRoomItemDict)) return false;
                        }
                    }
                    else if(resRoomItemDict.ContainsKey("ArrivalDate") &&
                        resRoomItemDict.ContainsKey("DepartureDate"))
                    {
                        DateTime arrivalDate = DateTime.Parse(resRoomItemDict["ArrivalDate"]);
                        DateTime departureDate = DateTime.Parse(resRoomItemDict["DepartureDate"]);
                        MatchCollection repeatResults = Regex.Matches(roomMatch.Value,
                            _XmlReader.ReadValue(resItem.Channel + "/Reg-Room/Reg-Repeats"));
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
                                    resRoomItemDict[node.Name] = GetRegexValue(node, repeatResults[i].Value, resItem.Channel));
                                if (!_Conn.InsertResRoom(resRoomItemDict)) return false;
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
                    if (!_Conn.InsertResRoom(resRoomItemDict)) return false;
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
                if (!String.IsNullOrEmpty(channel) && _XmlReader.ReadNode(channel) != null)
                    value = ConvertDateFromRegexValue(channel, value);
            }
            else if (innerText.StartsWith("Num"))
            {
                value = value.Replace("，", "").Replace(",", "");
            }
            _ErrorQueue.Enqueue(node.Name);
            return value;
        }

        private string ConvertDateFromRegexValue(string channel, string str)
        {
            str = str.Replace(",", "");
            foreach (string rStr in _XmlReader.ReadValue(channel + "/DateReplaceText").Split('|'))
            {
                if (!string.IsNullOrEmpty(rStr))
                {
                    int index = rStr.StartsWith(",") ? rStr.LastIndexOf(",") : rStr.IndexOf(",");
                    str = str.Replace(rStr.Substring(0, index), rStr.Substring(index + 1).Replace("null", "")).Trim();
                }
            }
            str = str.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            return String.IsNullOrEmpty(str) ? str : DateTime.ParseExact(str,
                _XmlReader.ReadValue(channel + "/DateFormatText").Split('|'),
                System.Globalization.DateTimeFormatInfo.InvariantInfo,
                System.Globalization.DateTimeStyles.None).ToString("s");
        }

        #endregion

        #region Xml

        public void UpdateResConfig()
        {
            _XmlReader.Close();
            _XmlReader = new ResXmlReader(_ResConfigFullPath);
            _XmlReader.Open();
            ResetRes();
        }

        public void OpenResConfig()
        {
            System.Diagnostics.Process.Start(_XmlReader.FullPath);
        }

        #endregion

        #region SHARED

        private static bool IsNumeric(string str)
        {
            return Int32.TryParse(str, out int a);
        }

        #endregion
        
        #region Res
        
        private void ResetRes()
        {
            tb_mainInfo.Text = string.Empty;
            //l_resDetails.Content = null;
            lb_resRooms.Items.Clear();
            dg_order_room.ItemsSource = null;
            tb_order_error.Text = string.Empty;
            tb_order_content.Text = string.Empty;
        }

        #endregion

        #region EmailTemplet

        public List<string> ReadEmailTempletList()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HROS\\EmailTemplates";
            if (!Directory.Exists(path)) return null;
            List<string> list = new List<string>();
            int startIndex = 0;
            int endIndex = 0;
            foreach (string filePath in Directory.GetFiles(path))
            {
                startIndex = filePath.LastIndexOf("\\") + 1;
                endIndex = filePath.LastIndexOf(".");
                list.Add(filePath.Substring(startIndex, endIndex - startIndex));
            }
            return list;
        }

        public void SetEmailTempletText(object sender, RoutedEventArgs e)
        {
            RibbonButton mi = sender as RibbonButton;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HROS\\EmailTemplates\\";
            path += mi.Label.ToString() + ".txt";
            FileStream fs = new FileStream(path, FileMode.Open);
            Clipboard.SetText(new StreamReader(fs).ReadToEnd());
            fs.Close();
            EMsgBox.ShowMessage("复制邮件模板成功！", mi.Parent as UIElement);
        }

        #endregion

        #region Staff

        public List<string> GetStaffList
        {
            get
            {
                List<string> staff = new List<string>(
                _XmlReader.ReadValue("Staff/RecStaff").Split(','));
                staff.Sort();
                return staff;
            }
        }

        #endregion

        #region Controls

        private void lb_order_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ELiteListBoxResItem res = rlv_res.SelectedResItem;
            if (res == null) return;
            ResetRes();
            #region DATAGRID

            lb_resRooms.Items.Add("全部");
            string sqlString = "select distinct Type from info_res_rooms where ResID='" + res.ID + "'";
            DataTable typeDT = _Conn.Select(sqlString);
            if (typeDT.Rows.Count > 0)
            {
                foreach (DataRow row in typeDT.Rows)
                {
                    lb_resRooms.Items.Add(row[0].ToString());
                }
                lb_resRooms.SelectedIndex = 0;
            }
            else
            {
                _ErrorQueue.Enqueue("未找到订单号为：" + res.ResNumber + "的房间数据。");
            }

            #endregion
            #region MAININFO

            sqlString = "SELECT " + _XmlReader.ReadValue("Main/ShowKeywords") +
                " FROM info_res,info_user WHERE ResNumber='" + res.ResNumber + "' and info_res.uid=info_user.uid";
            DataTable dst = _Conn.Select(sqlString);
            if (dst.Rows.Count < 1) return;
            string tmp = "";
            foreach (string title in new string[] { "FullName",
                    "ResNumber", "Email" })
            {
                tmp += "\r\n\r\n";
                if (dst.Columns.Contains(title))
                    tmp += dst.Rows[0][title].ToString();
            }
            tb_mainInfo.Text = tmp.Substring(4);

            #endregion
            #region COMMENT

            string[] disnames = _XmlReader.ReadValue("Main/EndorseKeywords-" +
                _XmlReader.ReadValue("Main/EndorseLanguage")).Split(',');
            string contentString = string.Empty;
            for (int i = 0; i <= dst.Columns.Count - 1; i++)
            {
                if (disnames[i] == "Room Fee")
                    contentString += "\r\n" + disnames[i] + ": " + ReadPriceString(res);
                else
                {
                    string itemStr = ItemToString(dst.Rows[0][i], false);
                    if (string.IsNullOrEmpty(itemStr)) continue;
                    contentString += "\r\n" + disnames[i] + ": " + itemStr;
                }
            }
            tb_order_content.Text = contentString.Substring(2);

            #endregion
        }

        private void lb_resRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ELiteListBoxResItem res = rlv_res.SelectedResItem;
            if (res == null)
            {
                ResetRes();
                return;
            }
            string type = lb_resRooms.SelectedValue as string;
            if (String.IsNullOrEmpty(type)) return;
            string sqlString = "SELECT ReservedDate,Type,Price,Persons,Rooms,Status" +
                " FROM info_res_rooms WHERE ResID='" + res.ID + "'";
            if (type != "全部")
                sqlString += " and Type='" + type + "'";
            DataTable dt = _Conn.Select(sqlString);
            if (dt.Rows.Count < 1)
            {
                _ErrorQueue.Enqueue("未找到订单号为：" + res.ResNumber + "且房型为：" + type + "的房间数据。");
            }
            else
            {
                DataTable gridDT = dt.Clone();
                foreach (DataColumn column in gridDT.Columns)
                    column.DataType = typeof(string);
                foreach (DataRow row in dt.Rows)
                {
                    DataRow newRow = gridDT.NewRow();
                    for (int ci = 0; ci < dt.Columns.Count; ci++)
                    {
                        newRow[ci] = ItemToString(row[ci], true);
                    }
                    gridDT.Rows.Add(newRow);
                }
                dg_order_room.ItemsSource = gridDT.DefaultView;
                dg_order_room.Columns[1].MaxWidth = 290;
            }
        }

        #endregion

        #region CONNECTION
        
        private string ReadPriceString(ELiteListBoxResItem res)
        {
            string sqlString = "select Price from info_res_rooms where ResID='" + res.ID + "'";
            DataTable dt = _Conn.Select(sqlString);
            if (dt.Rows.Count < 1) return string.Empty;
            string result = string.Empty;
            foreach (DataRow row in dt.Rows)
            {
                result += ',' + row[0].ToString();
            }
            if (result.Length > 1)
                return result.Substring(1);
            else
                return result;
        }

        private string ItemToString(object obj, bool isForGrid)
        {
            if (obj == null) return string.Empty;
            string result = string.Empty;
            if (obj.GetType() == typeof(DateTime))
            {
                if (DateTime.Now < _UnknownDate && ((DateTime)obj).Date == new DateTime(1997, 3, 3))
                {
                    result = ELiteConnection.DefaultDateStringBeforeLeave;
                }
                else
                {
                    if (isForGrid)
                        result = ((DateTime)obj).ToString("yyyy年MM月dd日");
                    else
                        result = ((DateTime)obj).ToString("yyyy/MM/dd");
                }
            }
            else
            {
                result = obj.ToString();
            }
            return result;
        }

        #endregion
        
        public string EmailText(string staffName)
        {
            ELiteListBoxResItem res = rlv_res.SelectedResItem;
            if (res == null) return null;
            return _ResHelper.BuildRoomDetails(_Conn, _XmlReader, res, staffName);
        }

        public void FindRes(string keyword = "")
        {
            bool isExisted = rlv_res.Find(keyword);
            EMsgBox.ShowMessage(isExisted ? "已" : "未" + "找到关键字为[" + keyword + "]的历史订单！");
        }
        
    }
}
