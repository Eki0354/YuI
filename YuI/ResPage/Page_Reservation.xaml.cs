﻿using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Data;
using System.IO;
using System.Xml;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using System.Windows.Controls.Ribbon;
using System.Threading;
using System.Windows.Threading;
using YuI.EControls;
using System.Data.SQLite;
using MementoConnection;
using MMC = MementoConnection.MMConnection;
using System.Windows;
using BookingElf;
using IOExtension;
using Ran;
using System.Threading.Tasks;
using static BookingElf.BubbleBookingListBox;
using FFElf;
using Newtonsoft.Json;

namespace YuI
{
    public enum CEs { None, Address, Theme, Body }

    public partial class Page_Reservation : Page
    {
        #region PROPERTY
        
        public APTXItem MementoAPTX
        {
            get { return (APTXItem)GetValue(MementoAPTXProperty); }
            set { SetValue(MementoAPTXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MementoAPTX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MementoAPTXProperty =
            DependencyProperty.Register("MementoAPTX", typeof(APTXItem), typeof(Page_Reservation), new PropertyMetadata(APTXItem.Master));
        
        public bool IsCERes { get; private set; } = false;
        BubbleBookingItemCollection _Bubbles;
        ConceivingBookingItem _AliveBubble;
        public EXmlReader _XmlReader { get; set; }
        FileSystemWatcher _HtmlFileSystemWatcher;
        Queue<string> _ErrorQueue = new Queue<string>();
        public string CommentString { get; private set; }
        public string CellString
        {
            get
            {
                BubbleBookingItem res = rlv_res.SelectedItem as BubbleBookingItem;
                if (res is null) return "未选中订单！";
                return _XmlReader.ReadValue(res.Channel + "/AbTitle") + "\r\n" + res.FullName;
            }
        }
        public string EmailTheme
        {
            get
            {
                string theme = _XmlReader.ReadValue(
                    (rlv_res.SelectedItem == null &&
                    (rlv_res.SelectedItem as BubbleBookingItem).Channel == "TrafficYouth") ?
                    "Email/EmailTheme-TrafficYouth" : "Email/EmailTheme");
                return theme.Replace("StaffName", this.MementoAPTX.Nickname);
            }
        }
        public string EmailAddress
        {
            get
            {
                if (string.IsNullOrEmpty(CommentString)) return string.Empty;
                int startIndex = CommentString.IndexOf("Email");
                if (startIndex < 0) return string.Empty;
                int endIndex = CommentString.IndexOf("\r\n", startIndex);
                return CommentString.Substring(startIndex + 7, endIndex - startIndex - 7);
            }
        }

        public OutlookEmail GetEmail(bool isHtml)
        {
            string address = EmailAddress;
            string theme = EmailTheme;
            string body = BuildRoomDetails(MementoAPTX.Nickname, isHtml);
            return (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(theme) ||
                string.IsNullOrEmpty(body)) ?
                null : new OutlookEmail(address, theme, body);
        }

        public BubbleBookingItem ActiveResItem => rlv_res.SelectedItem as BubbleBookingItem;
        DateTime _UnknownDate => DateTime.Parse(_XmlReader.ReadValue("Main/UnknownDate"));

        #endregion

        #region Itinialize

        public Page_Reservation()
        {
            InitializeComponent();
            if (File.Exists(MementoPath.ResConfigPath))
            {
                _XmlReader = new EXmlReader(MementoPath.ResConfigPath);
                _XmlReader.Open();
            }
            else
            {
                MainWindow.Pop("配置文件不存在！\r\n请检查是否已被删除。", "提示");
                Environment.Exit(0);
            }
            if (_XmlReader.ReadValue("Main/Enable") == "0") GetNPC();
            InitializeControls();
            MMC.FormatResRoomDate();
            _Bubbles = BubbleBookingItemCollection.FromDataTable(MMC.UnCheckedBubbleResSet());
            rlv_res.Bubbles = _Bubbles;
        }

        private void Page_Initialized(object sender, EventArgs e)
        {
            rlv_res.SelectionChanged += lb_order_SelectionChanged;
            rlv_res.DeleteMenuItemClicked += MenuDeleteBooking_Click;
            rlv_res.HideSelectionsMenuItemClicked += MenuHideBooking_Click;
            rlv_res.HideAllItemsMenuItemClicked += MenuHideBooking_Click;
            rlv_res.MarkSelectionsMenuItemClicked += MenuMarkBooking_Click;
            rlv_res.MarkAllItemsMenuItemClicked += MenuMarkBooking_Click;
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
                MainWindow.Pop(_XmlReader.ReadValue("Main/ErrorText"), "提示");
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
            _HtmlFileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _HtmlFileSystemWatcher.Changed += _HtmlFileSystemWatcher_Changed;
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

        private void MenuDeleteBooking_Click(object sender, BubbleBookingListBox.BubblesChangedEventArgs e)
        {
            e.ChangedBubbles.ForEach(item => MMC.DeleteResByResNumber(item.ResNumber));
        }

        private void MenuHideBooking_Click(object sender, BubbleBookingListBox.BubblesChangedEventArgs e)
        {
            MMCBat(new Action(() => e.ChangedBubbles.ForEach(
                item => MMC.UpdateResChecked(item.ResNumber))));
        }

        private void MenuMarkBooking_Click(object sender, BubbleBookingListBox.BubblesChangedEventArgs e)
        {
            MMCBat(new Action(() => e.ChangedBubbles.ForEach(
                item =>
                {
                    if (item.State == BubbleBookingState.Checked)
                    {
                        MMC.UpdateResChecked(item.ResNumber, true);
                    }
                    else
                    {
                        MMC.UpdateResChecked(item.ResNumber, false);
                        MMC.UpdateResState(item.ResNumber, (int)item.State);
                    }
                })));
        }

        private void MMCBat(Action operation)
        {
            Action action = new Action(() => MMC.Bat(operation));
            action.BeginInvoke((result) =>
            {
                action.EndInvoke(result);
                this.Dispatcher.Invoke(new Action(() =>
                {
                    MainWindow.Pop("呼噜", "数据库同步完成！");
                }));
            }, null);
        }

        #endregion

        #region GETRES

        public void SniffRes()
        {
            string text = null;
            if (!Clipboard.ContainsText()) return;
            bool isGetOK = false;
            int getTimes = 0;
            while (!isGetOK && getTimes < 3)
            {
                text = string.Empty;
                try
                {
                    text = Clipboard.GetText();
                }
                catch
                {

                }
                isGetOK = !string.IsNullOrEmpty(text);
                getTimes++;
                Thread.Sleep(100);
            }
            if (string.IsNullOrEmpty(text)) return;
            if (text.StartsWith("MementoRes"))
            {
                text = text.Substring("MementoRes=>".Length);
                GetBookingRes(JsonConvert.DeserializeObject<BookingResItem>(text));
            }
            else
            {
                foreach (string channel in _XmlReader.ReadValue("Main/Clip-WebSite").Split(','))
                {
                    if (text.IndexOf(_XmlReader.ReadValue(channel + "/ClipboardKeyword")) < 0) continue;
                    _AliveBubble = new ConceivingBookingItem() { Channel = channel };
                    if (_XmlReader.ReadValue(channel + "/IsNet") != "0")
                    {
                        _AliveBubble.FullName = GetRegexValue(_XmlReader.ReadNode(channel + "/Reg-ResBalloon/FullName"), text);
                        _AliveBubble.ResNumber = GetRegexValue(_XmlReader.ReadNode(channel + "/Reg-ResBalloon/ResNumber"), text);
                        text = GetHttpHtml("HostelWorld", _AliveBubble.ResNumber);
                    }
                    GetRes(channel, text);
                    Clipboard.SetText(string.Empty);
                    break;
                }
            }
        }

        public void GetBookingRes(BookingResItem resItem)
        {
            _AliveBubble = new ConceivingBookingItem()
            {
                Channel = "Booking",
                ResNumber = resItem.ResInfo.ResNumber,
                UID = MMC.InsertUser(resItem.UserInfo.ToDictionary())
            };
            if (_AliveBubble.UID == -1) { _AliveBubble = null; return; }
            var resDict = resItem.ResInfo.ToDictionary();
            resDict.Add("uid", _AliveBubble.UID);
            resDict.Add("Channel", _AliveBubble.Channel);
            int resResult = MMC.IsResExisted(_AliveBubble.ResNumber) ? 2 :
                (MMC.InsertRes(resDict) ? 0 : 1);
            switch (resResult)
            {
                case 1:
                    MMC.DeleteResByResNumber(_AliveBubble.ResNumber);
                    Ran.MainWindow.SaveErrorLog(_AliveBubble.ToString());
                    break;
                case 2:
                    Dispatcher.Invoke(new Action(() => FindRes(_AliveBubble.ResNumber)));
                    break;
                default:
                    /*_AliveBubble.ID = MMC.GetItem<long>("select id from info_res where ResNumber='" +
                        _AliveBubble.ResNumber + "'") ?? -1;*/
                    if (GetBookingRes_ResRoomItems(resItem))
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            FindRes(_AliveBubble.ResNumber);
                            rlv_res.SelectedIndex = 0;
                        }));
                        _ErrorQueue.Enqueue("获取完毕！已加入左侧订单列表 . . .");
                    }
                    else
                    {
                        MMC.DeleteResByResNumber(_AliveBubble.ResNumber);
                        Ran.MainWindow.SaveErrorLog(_AliveBubble.ToString());
                        _ErrorQueue.Enqueue("获取房间出错！请检查错误日志 . . .");
                    }
                    break;
            }
            _AliveBubble = null;
        }

        public bool GetBookingRes_ResRoomItems(BookingResItem resItem)
        {
            bool result = true;
            foreach(var ri in resItem.RoomInfo)
            {
                foreach(var dict in ri.ToDictionaryArry())
                {
                    var uid = MMC.GetUIDByFullName((string)dict["FullName"]);
                    dict.Add("ResNumber", _AliveBubble.ResNumber);
                    dict.Add("uid", uid);
                    dict.Remove("FullName");
                    result = MMC.InsertResRoom(dict);
                    if (!result) break;
                }
                if (!result) break;
            }
            return result;
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
                    Ran.MainWindow.SaveErrorLog(_AliveBubble.ToString());
                    break;
                case 2:
                    Dispatcher.Invoke(new Action(() => FindRes(_AliveBubble.ResNumber)));
                    break;
                default:
                    /*_AliveBubble.ID = MMC.GetItem<long>("select id from info_res where ResNumber='" +
                        _AliveBubble.ResNumber + "'") ?? -1;*/
                    if (GetRes_ResRoomItems(htmlText))
                    {
                        Dispatcher.Invoke(new Action(() => 
                        {
                            FindRes(_AliveBubble.ResNumber);
                            rlv_res.SelectedIndex = 0;
                        }));
                        _ErrorQueue.Enqueue("获取完毕！已加入左侧订单列表 . . .");
                    }
                    else
                    {
                        MMC.DeleteResByResNumber(_AliveBubble.ResNumber);
                        Ran.MainWindow.SaveErrorLog(_AliveBubble.ToString());
                        _ErrorQueue.Enqueue("获取房间出错！请检查错误日志 . . .");
                    }
                    break;
            }
            _AliveBubble = null;
        }

        public void GetHWRes()
        {
            string orderNumber = string.Empty;
            if (string.IsNullOrEmpty(orderNumber)) orderNumber = Interaction.InputBox(
                 "请输入订单号：", "订单号", "").Replace("89968-", "").Replace(" ", "");
            if (string.IsNullOrEmpty(orderNumber)) return;
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
            foreach (XmlNode node in _XmlReader.ReadNodes(_AliveBubble.Channel + "/Reg-User"))
            {
                userItemDict.Add(node.Name, GetRegexValue(node, text, _AliveBubble.Channel));
            }
            if (userItemDict.ContainsKey("FullName"))
            {
                _AliveBubble.FullName = userItemDict["FullName"] as string;
            }
            else
            {
                if (String.IsNullOrEmpty(_AliveBubble.FullName) || 
                    _AliveBubble.FullName == ELiteProperties.DefaultUserName)
                {
                    string fullName = CombineFullName(userItemDict);
                    _AliveBubble.FullName = fullName.Length > 0 ? fullName : ELiteProperties.DefaultUserName;
                }
                userItemDict.Add("FullName", _AliveBubble.FullName);
            }
            return MMC.InsertUser(userItemDict);
        }

        private string CombineFullName(Dictionary<string, object> itemDict)
        {
            string result = string.Empty;
            foreach(string nameKey in new string[] { "Surname", "MiddleName", "GivenName" })
            {
                if (itemDict.ContainsKey(nameKey)) result += " " + itemDict[nameKey];
            }
            return result.Length > 0 ? result.Substring(1) : result;
        }

        /// <summary> 返回值：0-新订单保存成功；1-新订单保存失败；2-已存在订单 </summary>
        private int GetRes_ResItems(string text)
        {
            Dictionary<string, object> resItemDict = new Dictionary<string, object>();
            _XmlReader.ReadNodes(_AliveBubble.Channel + "/Reg-Res").ForEach(node =>
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
            foreach (Match roomMatch in Regex.Matches(text, _XmlReader.ReadValue(_AliveBubble.Channel + "/Reg-Rooms")))
            {
                Dictionary<string, object> resRoomItemDict = new Dictionary<string, object>
                {
                    { "ResNumber", _AliveBubble.ResNumber },
                    { "uid", _AliveBubble.UID },
                };
                List<XmlNode> nodeList = _XmlReader.ReadNodes(_AliveBubble.Channel + "/Reg-Room");
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
                nodeList = _XmlReader.ReadNodes(_AliveBubble.Channel + "/Reg-Room/Reg-Repeat");
                if (nodeList.Count > 0)//同一房间不同日期、价格等
                {
                    nodeList.ForEach(node => resRoomItemDict.Add(node.Name, ""));
                    if (resRoomItemDict.ContainsKey("ReservedDate"))
                    {
                        foreach (Match repeatMatch in Regex.Matches(roomMatch.Value,
                            _XmlReader.ReadValue(_AliveBubble.Channel + "/Reg-Room/Reg-Repeats")))
                        {
                            nodeList.ForEach(node =>
                                resRoomItemDict[node.Name] = GetRegexValue(node, repeatMatch.Value, _AliveBubble.Channel));
                            if (!MMC.InsertResRoom(resRoomItemDict)) return false;
                        }
                    }
                    else if(resRoomItemDict.ContainsKey("ArrivalDate") &&
                        resRoomItemDict.ContainsKey("DepartureDate"))
                    {
                        Console.WriteLine(resRoomItemDict["ArrivalDate"].GetType());
                        DateTime arrivalDate = DateTime.Parse(resRoomItemDict["ArrivalDate"] as string);
                        DateTime departureDate = DateTime.Parse(resRoomItemDict["DepartureDate"] as string);
                        MatchCollection repeatResults = Regex.Matches(roomMatch.Value,
                            _XmlReader.ReadValue(_AliveBubble.Channel + "/Reg-Room/Reg-Repeats"));
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
            _XmlReader = new EXmlReader(MementoPath.ResConfigPath);
            _XmlReader.Open();
            ResetRes();
        }

        #endregion

        #region SHARED

        private static bool IsNumeric(string str)
        {
            return Int32.TryParse(str, out int a);
        }

        #endregion

        #region Controls

        private void ResetRes()
        {
            //l_resDetails.Content = null;
            lvRes.Visibility = Visibility.Collapsed;
            lb_resRooms.Items.Clear();
            dg_order_room.ItemsSource = null;
            CommentString = string.Empty;
        }

        private void lb_order_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BubbleBookingItem res = rlv_res.SelectedItem as BubbleBookingItem;
            ResetRes();
            if (res == null) return;
            #region DATAGRID

            lb_resRooms.Items.Add("全部");
            string sqlString = "select distinct Type from info_res_rooms where ResNumber='" + res.ResNumber + "'";
            DataTable typeDT = MMC.Select(sqlString);
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
            #region COMMENT

            sqlString = "SELECT * FROM info_res,info_user WHERE ResNumber='" + res.ResNumber + "' and info_res.uid=info_user.uid";
            DataTable dst = null;
            try
            {
                dst = MMC.Select(sqlString);
                if (dst is null || dst.Rows.Count < 1) return;
            }
            catch
            {
                MMC.DeleteResByResNumber(res.ResNumber);
                Dispatcher.Invoke(() =>
                {
                    rlv_res.Bubbles.RemoveAt(rlv_res.Bubbles.IndexOfResNumber(res.ResNumber));
                });
                Clipboard.SetText(string.Format("{0}-{1}", res.Channel, res.ResNumber));
                MainWindow.Pop(string.Format("来源：{0}\r\n订单号：{1}\r\n" +
                    "此订单信息有误，已删除。信息已复制到剪贴板，请重新获取。",
                    res.Channel, res.ResNumber));
                return;
            }
            Dictionary<string, object> infos = new Dictionary<string, object>();
            for (int i = 0; i < dst.Columns.Count - 1; i++)
            {
                infos.Add(dst.Columns[i].Caption, dst.Rows[0][i]);
            }
            lvRes.ItemsSource = infos;
            IsCERes = infos["Email"] is string email && email.Length > 0;
            sqlString = string.Format("SELECT {0} FROM info_res,info_user WHERE " +
                "ResNumber='{1}' and info_res.uid=info_user.uid",
                _XmlReader.ReadValue("Main/ShowKeywords"), res.ResNumber);
            dst = MMC.Select(sqlString);
            if (dst is null || dst.Rows.Count < 1) return;
            string[] infonames = _XmlReader.ReadValue("Main/EndorseKeywords-" +
                _XmlReader.ReadValue("Main/EndorseLanguage")).Split(',');
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
            CommentString = contentString.Substring(2);

            #endregion
        }

        private void lb_resRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BubbleBookingItem res = rlv_res.SelectedItem as BubbleBookingItem;
            if (res == null)
            {
                ResetRes();
                return;
            }
            string type = lb_resRooms.SelectedValue as string;
            if (String.IsNullOrEmpty(type)) return;
            string sqlString = "SELECT ReservedDate,Type,Price,Persons,Rooms,Status" +
                " FROM info_res_rooms WHERE ResNumber='" + res.ResNumber + "'";
            if (type != "全部")
                sqlString += " and Type='" + type + "'";
            DataTable dt = MMC.Select(sqlString);
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
            }
        }

        #endregion

        #region MMCECTION
        
        private string ReadPriceString(BubbleBookingItem res)
        {
            string sqlString = "select Price from info_res_rooms where ResNumber='" + res.ResNumber + "'";
            DataTable dt = MMC.Select(sqlString);
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
                if (((DateTime)obj).Date == ELiteProperties.BirthdayOfMori)//DateTime.Now < _UnknownDate &&
                {
                    var unme = new Random();
                    if (unme.Next(18) > 2)
                        result = ELiteProperties.BirthDayStringBeforeJudgement;
                }
                else
                {
                    result = ((DateTime)obj).ToString(
                        isForGrid ? "yyyy年MM月dd日" : "yyyy/MM/dd");
                }
            }
            else
            {
                result = obj.ToString();
            }
            return result;
        }

        #endregion

        #region SearchBubble
        
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            FindRes();
        }

        public void FindRes(string keyword = null)
        {
            if (keyword is null)
                keyword = Interaction.InputBox("请输入要查找的关键词（订单号或客人姓名）：", "提示");
            if (string.IsNullOrEmpty(keyword)) return;
            BubbleBookingItemCollection items = BubbleBookingItemCollection.FromDataTable(
                MMC.FindResByResNumberOrFullName(keyword), true);
            Dispatcher.Invoke(new Action(() =>
            {
                int index = -1;
                foreach (BubbleBookingItem item in items)
                {
                    index = rlv_res.FindByResNumber(item.ResNumber);
                    if (index < 0)
                    {
                        _Bubbles.AddToFirst(item);
                        index = 0;
                    }
                }
                if (items.Count > 0)
                {
                    if (index > -1)
                    {
                        rlv_res.SelectedIndex = 1;
                        rlv_res.SelectedIndex = index;
                    }
                    MainWindow.Pop("成功！", string.Format(
                        "已找到关键字为[{0}]的历史订单！", keyword));
                }
                else
                {
                    MainWindow.Pop("失败！", string.Format(
                        "未找到关键字为[{0}]的历史订单！", keyword));
                }
            }));
        }
        
        #endregion
        
        public void EmailSendButton_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
                MainWindow.Pop("咕咚", "开始发送邮件. . .")));
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (this.SendMail())
                    MainWindow.Pop("叮咚", "邮件发送成功！");
                else
                    MainWindow.Pop("哦豁", "邮件发送失败！");
            }));
        }

        private void HWButton_Click(object sender, RoutedEventArgs e)
        {
            GetHWRes();
        }

        public void SetResDetailsCopy()
        {
            Clipboard.SetText(CellString + ";;;" + CommentString +
                "\r\n\r\n" + this.MementoAPTX.Nickname +
                " " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        }
    }
}
