using System;
using System.Collections.Generic;
using System.Windows.Controls;
using ELite.Reservation;
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
using Interface_Reception_Ribbon.PopupMsg;
using System.Threading;
using System.ComponentModel;

namespace Interface_Reception_Ribbon
{
    public partial class Page_Reservation : Page
    {
        #region PROPERTY

        private string _LocalPath = Environment.CurrentDirectory;
        //public ListBox ResListBox { get { return lb_order; } }
        //private ListBoxResItem _CurrentRes;
        //string DisplayText;
        //bool _isSad = false;
        string LocalPath = Environment.CurrentDirectory;
        string WebOrderNumber;
        bool IsLogined = false;
        string HttpWebSite;
        CookieContainer Cookies;
        HttpWebRequest httpReq;
        EkiXmlDocument.EXmlReader _XmlReader;
        FileSystemWatcher _HtmlFileSystemWatcher;
        //ELiteConnection _Conn = MainWindow._Conn;
        OleDbConnection _Conn;
        OleDbCommand _Comm = new OleDbCommand();
        public string CommentString => tb_order_content.Text;
        public string CellString
        {
            get
            {
                object obj = lb_order.SelectedItem;
                if (obj is null) return string.Empty;
                ListBoxResItem res = obj as ListBoxResItem;
                int index = -1;
                foreach (string ct in _XmlReader.ReadValue("Main/Website").Split(','))
                {
                    index++;
                    if (ct == res.Channel)
                        return _XmlReader.ReadValue("Main/Website-s").Split(',')[index] +
                            "\r\n" + res.GuestName;
                }
                return string.Empty;
            }
        }
        public string EmailTheme => _XmlReader.ReadValue("Email/EmailTheme");
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
        EmailHelper.EmailHelper _EmailHelper;
        [BrowsableAttribute(false)]
        [IODescriptionAttribute("FSW_SynchronizingObject")]
        public ISynchronizeInvoke SynchronizingObject { get; set; }

        #endregion

        public Page_Reservation()
        {
            InitializeComponent();
            if (!LocalPath.EndsWith("\\")) LocalPath += "\\";
            if (File.Exists(LocalPath + "Config.xml"))
            {
                _XmlReader = new EkiXmlDocument.EXmlReader(LocalPath, "Config");
                _XmlReader.Open();
            }
            else
            {
                MsgHelper.ShowMessage("配置文件不存在！\r\n请检查是否已被删除。", "提示");
                Environment.Exit(0);
            }
            _Conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                LocalPath + "Mrs Panda.mdb;User ID=Admin;Jet OLEDB:Database Password=" +
                "Eki20150613");
            _Conn.Open();
            _Comm.Connection = _Conn;
            if (_XmlReader.ReadValue("Main/Enable") == "0") ShowError();
            string SQL = "SELECT WebSite,OrderNumber,CustomerName FROM Main WHERE Checked=0 ORDER BY [ID]";
            DataTable dst = Select(SQL);
            if (dst.Rows.Count > 0)
            {
                for (int i = 0; i <= dst.Rows.Count - 1; i++)
                {
                    lb_order.Items.Add(new ListBoxResItem(dst.Rows[i][0].ToString(),
                        dst.Rows[i][1].ToString(), dst.Rows[i][2].ToString()));
                }
            }
            EditControls();
            _EmailHelper = new EmailHelper.EmailHelper();
        }

        private void ShowError()
        {
            DateTime dt = DateTime.Now;
            if (dt.Date.Month != 2 && (dt.Day != 14 || dt.Day != 15))
            {
                MsgHelper.ShowMessage(_XmlReader.ReadValue("Main/ErrorText"), "提示");
                Environment.Exit(0);
            }
        }

        private void EditControls()
        {
            _HtmlFileSystemWatcher = new FileSystemWatcher();
            _HtmlFileSystemWatcher.EndInit();
            _HtmlFileSystemWatcher.Path = LocalPath + "Html";
            _HtmlFileSystemWatcher.Filter = "*.htm";
            _HtmlFileSystemWatcher.EnableRaisingEvents = true;
            //_HtmlFileSystemWatcher.SynchronizingObject = SynchronizingObject;
            _HtmlFileSystemWatcher.Changed += _HtmlFileSystemWatcher_Changed;
        }

        public void ReadResFromClipboard(string tmp)
        {
            foreach (string s in _XmlReader.ReadValue("Main/Clip-WebSite").Split(','))
            {
                if (tmp.IndexOf(s.Substring(s.IndexOf("=") + 1)) > -1)
                {
                    GetRev(s.Substring(0, s.IndexOf("=")),
                        tmp.Replace("\rCustomer Last Name 顾客姓氏	", " "));
                }
            }
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
                    HttpWebSite = new string[] { "Booking", "TrafficYouth" }[index];
                    GetRev(HttpWebSite, htmlString);
                    break;
                }
            }
        }

        public void GetHWRes()
        {
            string ordernumber = Convert.ToString(Interaction.InputBox(
                "请输入订单号：", "订单号", "").Replace("89968-", "").Replace(" ", ""));
            if (!string.IsNullOrEmpty(ordernumber))
            {
                if (IsNumeric(ordernumber) == true)
                {
                    if (ExistRes(ordernumber))
                    {
                        FindOrder(ordernumber);
                        return;
                    }
                    HttpWebSite = "HostelWorld";// sender.ToString().Replace("M-", "");
                    WebOrderNumber = ordernumber;
                    GetRev(HttpWebSite, ResGetter.GetHWRes(ordernumber));
                }
                else
                {
                    AppenErrorText("错误的订单号：" + ordernumber + "！\r\n");
                }
            }
        }

        private void GetOrder()
        {
            if (IsLogined == false)
            {
                AppenErrorText("正在登录网站 . . ." + "\r\n");
                Cookies = new CookieContainer();
                httpReq = (HttpWebRequest)(WebRequest.Create(
                    new Uri(_XmlReader.ReadValue(HttpWebSite + "/LoginSite"))));
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
                AppenErrorText("正在POST网站对象 . . ." + "\r\n");
                httpReq.Method = "POST";
                byte[] PostByte = System.Text.Encoding.GetEncoding("GB2312").GetBytes(
                    _XmlReader.ReadValue(HttpWebSite + "/PostString"));
                httpReq.ContentLength = PostByte.Length;
                Stream PostStream = httpReq.GetRequestStream();
                PostStream.Write(PostByte, 0, PostByte.Length);
                PostStream.Close();
                Cookies = httpReq.CookieContainer;
                HttpWebResponse httpRes = (HttpWebResponse)(httpReq.GetResponse());
                foreach (Cookie ck in httpRes.Cookies)
                {
                    Cookies.Add(ck);
                }
                IsLogined = true;
            }
            httpReq = (HttpWebRequest)(WebRequest.Create(new Uri(_XmlReader.ReadValue(
                HttpWebSite + "/RevSite").Replace("OrderNumber", WebOrderNumber))));
            httpReq.CookieContainer = Cookies;
            httpReq.Method = "GET";
            AppenErrorText("正在获取订单页面html代码 . . ." + "\r\n");
            HttpWebResponse httpResp = (HttpWebResponse)(httpReq.GetResponse());
            StreamReader reader = new StreamReader(httpResp.GetResponseStream(),
                System.Text.Encoding.GetEncoding("GB2312"));
            string respHTML = reader.ReadToEnd();
            reader.Close();
            if (respHTML.IndexOf(_XmlReader.ReadValue(HttpWebSite + "/ConfirmKeywords")) > -1)
            {
                AppenErrorText("订单未确认 . . ." + "\r\n");
                //AppendText &="新订单！正在确认 . . ." & vbCrLf
                //httpReq = DirectCast(WebRequest.Create(New Uri(_XmlReader.ReadValue(HttpWebSite & "/ConfirmSite"))), HttpWebRequest)
                //PostWeb(_XmlReader.ReadValue(HttpWebSite & "/ConfirmString") & WebOrderNumber)
                //respHTML = GetWeb()
            }
            if (!(httpReq == null))
            {
                httpReq.GetResponse().Close();
            }
            httpReq = null;
            IsLogined = false;
            GetRev(HttpWebSite, respHTML);
        }

        private void AppenErrorText(string errorText)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                tb_order_error.AppendText(errorText);
            }));
        }

        public void GetRev(string WebSite, string HtmlText)
        {
            AppenErrorText("代码获取完毕！正在读取详情 . . ." + "\r\n");
            string keywords = _XmlReader.ReadValue(WebSite + "/RevKeywords");
            string ItemName = "[WebSite]";
            ListBoxResItem NewOrder = new ListBoxResItem(WebSite, "");
            int WebIndex = Convert.ToInt32(_XmlReader.ReadValue(WebSite + "/Index"));
            string ItemValue = "\'" + WebSite + "\'";
            foreach (XmlNode Node in _XmlReader.ReadNodes(WebSite + "/Regex-Order"))
            {
                switch (Node.Name)
                {
                    case "Date":
                        foreach (XmlNode N in Node.ChildNodes)
                        {
                            string tmp = Regex.Match(HtmlText, N.InnerText).Value;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                ItemName += ",[" + N.Name + "]";
                                ItemValue += ",#" + ConvertDateFromHtml(WebSite,
                                    Regex.Match(HtmlText, N.InnerText).Value) + "#";
                                AppenErrorText("\r\n" + N.Name);
                            }
                        }
                        break;
                    case "Number":
                        foreach (XmlNode N in Node.ChildNodes)
                        {
                            string tmp = Regex.Match(HtmlText, N.InnerText).Value;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                ItemName += ",[" + N.Name + "]";
                                ItemValue += "," + Regex.Match(HtmlText, N.InnerText)
                                    .Value.Replace(",", "").Replace(" ", "");
                                AppenErrorText("\r\n" + N.Name);
                            }
                        }
                        break;
                    case "String":
                        foreach (XmlNode N in Node.ChildNodes)
                        {
                            string tmp = Regex.Match(HtmlText, N.InnerText).Value;
                            if (!string.IsNullOrEmpty(tmp))
                            {
                                ItemName += ",[" + N.Name + "]";
                                AppenErrorText("\r\n" + N.Name);
                                switch (N.Name)
                                {
                                    case "OrderNumber":
                                        NewOrder.ResNumber = tmp;
                                        if (ExistRes(NewOrder.ResNumber))
                                        {
                                            FindOrder(NewOrder.ResNumber);
                                            return;
                                        }
                                        ItemValue += ",\'" + NewOrder.ResNumber + "\'";
                                        break;
                                    case "PhoneNumber":
                                        ItemValue += ",\'" + tmp.Replace(" ", "") + "\'";
                                        break;
                                    case "Status":
                                        if (string.IsNullOrEmpty(tmp))
                                        {
                                            ItemValue += ",\'UnConfirmed\'";
                                        }
                                        else
                                        {
                                            ItemValue += ",\'Confirmed\'";
                                        }
                                        break;
                                    case "CustomerName":
                                        ItemValue += ",\'" + tmp.Replace("ms ", "").Replace("mr ", "").Replace("mrs ", "").Replace("Customer Last Name 顾客姓氏", "").Replace("\r\n", "").Replace(Convert.ToString("\t"), "") + "\'";
                                        break;
                                    default:
                                        ItemValue += ",\'" + tmp + "\'";
                                        break;
                                }
                                if (N.Name == "GuestName")
                                    NewOrder.GuestName = tmp;
                            }
                        }
                        break;
                }
            }
            if (EditDataBase("INSERT INTO Main (" + ItemName + ") VALUES (" + ItemValue + ")", "保存订单详情") == false)
            {
                return;
            }
            foreach (Match m in Regex.Matches(HtmlText, _XmlReader.ReadValue(WebSite + "/Reg-Rooms")))
            {
                ItemName = "OrderNumber";
                ItemValue = NewOrder.ResNumber;
                foreach (XmlNode Node in _XmlReader.ReadNodes(WebSite + "/Regex-Room"))
                {
                    switch (Node.Name)
                    {
                        case "Date":
                            foreach (XmlNode N in Node.ChildNodes)
                            {
                                if (Regex.Match(m.Value, N.InnerText).Value != "")
                                {
                                    ItemName += "," + N.Name;
                                    ItemValue += ",#" + ConvertDateFromHtml(WebSite,
                                        Regex.Match(m.Value, N.InnerText).Value) + "#";
                                    AppenErrorText("\r\n" + N.Name);
                                }
                            }
                            break;
                        case "Number":
                            foreach (XmlNode N in Node.ChildNodes)
                            {
                                if (Regex.Match(m.Value, N.InnerText).Value != "")
                                {
                                    AppenErrorText("\r\n" + N.Name);
                                    ItemName += "," + N.Name;
                                    string num = Convert.ToString(Regex.Match(m.Value,
                                        N.InnerText).Value.Replace(",", "").Replace(" ", ""));
                                    if (N.Name == "Persons" && (string.IsNullOrEmpty(num) || num == "0"))
                                    {
                                        num = "1";
                                    }
                                    ItemValue += "," + num;
                                }
                            }
                            break;
                        case "String":
                            foreach (XmlNode N in Node.ChildNodes)
                            {
                                string tmp = Regex.Match(m.Value, N.InnerText).Value;
                                if (!string.IsNullOrEmpty(tmp))
                                {
                                    ItemName += "," + N.Name;
                                    AppenErrorText("\r\n" + N.Name);
                                    if (WebSite == "Booking" || WebSite == "TrafficYouth")
                                    {
                                        if (N.Name == "Price")
                                        {
                                            string Value = string.Empty;
                                            foreach (Match pm in Regex.Matches(m.Value, N.InnerText))
                                            {
                                                Value += "," + pm.Groups[0].Value;
                                            }
                                            ItemValue += ",\'" + Value.Substring(1) + "\'";
                                        }
                                        else if (N.Name == "Status")
                                        {
                                            string Value = Regex.Match(m.Value, N.InnerText).Value;
                                            if (string.IsNullOrEmpty(Value))
                                            {
                                                Value = "Ok";
                                            }
                                            else
                                            {
                                                Value = "Cancelled";
                                            }
                                            ItemValue += ",\'" + Value + "\'";
                                        }
                                        else
                                        {
                                            ItemValue += ",\'" + Regex.Match(m.Value, N.InnerText).Value + "\'";
                                        }
                                    }
                                    else
                                    {
                                        ItemValue += ",\'" + Regex.Match(m.Value, N.InnerText).Value + "\'";
                                    }
                                }
                            }
                            break;
                    }
                }
                if (EditDataBase("INSERT INTO " + WebSite + "(" + ItemName + ") VALUES (" + ItemValue + ")", "保存房型详情") == false)
                {
                    EditDataBase("DELETE FROM Main WHERE OrderNumber='" + NewOrder.ResNumber + "'", "删除订单详情");
                    return;
                }
            }
            AppenErrorText("获取完毕！已加入左侧订单列表 . . ." + "\r\n");
            Dispatcher.BeginInvoke((Action)delegate
            {
                lb_order.Items.Add(NewOrder);
                lb_order.SelectedItem = NewOrder;
                lb_order.ScrollIntoView(NewOrder);
            });
        }

        private string ConvertDateFromHtml(string WebSite, string Str)
        {
            Str = Str.Replace(",", "");
            foreach (string RStr in _XmlReader.ReadValue(WebSite + "/DateReplaceText").Split('|'))
            {
                if (!string.IsNullOrEmpty(RStr))
                {
                    Str = Str.Replace(RStr.Substring(0, RStr.IndexOf(",")),
                        RStr.Substring(RStr.IndexOf(",") + 1).Replace("null", "")).Trim();
                }
            }
            Str = Str.Replace(Constants.vbLf, "").Replace(Constants.vbCr, "").Replace("\t", "").Replace(",", "");
            return DateTime.ParseExact(Str,
                _XmlReader.ReadValue(WebSite + "/DateFormatText").Split('|'),
                System.Globalization.DateTimeFormatInfo.InvariantInfo,
                System.Globalization.DateTimeStyles.None).ToString();
        }

        public void copyroomstatusb_Click(object sender, EventArgs e)
        {
            复制房态ToolStripMenuItem_Click(null, null);
        }

        public void 复制房态ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cn = tb_order_content.Text;
            string[] shortws = _XmlReader.ReadValue("Main/Website-s").Split(',');
            if (!string.IsNullOrEmpty(cn))
            {
                string[] tmp = _XmlReader.ReadValue("Main/Website").Split(',');
                for (int i = 0; i <= tmp.Length - 1; i++)
                {
                    if (lb_order.SelectedItem.ToString() == tmp[i])
                    {
                        (new Microsoft.VisualBasic.Devices.Computer()).Clipboard.SetText(
                            shortws[i] + "\r\n" + cn.Substring(0, cn.IndexOf("\r\n")));
                        break;
                    }
                }
            }
            else
            {
                Interaction.MsgBox("客人姓名不能为空！", MsgBoxStyle.OkOnly, "警告");
            }
        }

        private string BuildRoomDetails()
        {
            ListBoxResItem obj = lb_order.SelectedItem as ListBoxResItem;
            string RoomTypes = null;
            int rcount = 0;
            string[] RT = null;
            int[,] Rs = new int[3, 1];
            DateTime[,] RD = new DateTime[2, 1];
            int rtindex = 0;
            string WebSite = lb_order.SelectedItem.ToString();
            int rtc = dg_order_room.Columns.Count;
            string[] d = _XmlReader.ReadValue("Email/Day").Split(',');
            string[] m = _XmlReader.ReadValue("Email/" + _XmlReader.ReadValue("Email/MonthType")).Split(',');
            string tmpstr = string.Empty;
            string[] cs = _XmlReader.ReadValue("Email/Count").Split(',');
            bool IsSingleOrder = true;
            foreach (XmlNode n in _XmlReader.ReadNodes("RoomType"))
            {
                if (n.NodeType == XmlNodeType.Element)
                {
                    Array.Resize(ref RT, rcount + 1);
                    RT[rcount] = n.InnerText;
                    rcount++;
                }
            }
            rcount = 0;
            if (_XmlReader.ReadValue("Main/Local-WebSite").IndexOf(WebSite) > -1)
            {
                string SQL = "SELECT DISTINCT ArrivalDate,DepartureDate,RoomType,Persons,Price,Nights FROM " + obj.Channel + " WHERE Status<>\'Cancelled\' AND OrderNumber=\'" + obj.ResNumber + "\'";
                DataTable roomt = Select(SQL);
                foreach (DataRow dsrow in roomt.Rows)
                {
                    for (int i = 0; i <= RT.Length - 1; i++)
                    {
                        if (RT[i].IndexOf(dsrow[2].ToString()) > -1)
                        {
                            rtindex = i;
                            break;
                        }
                    }
                    /*rcount = (Convert.ToInt32(dsrow[3].ToString() + RT[rtindex]
                        .Substring(0, 1)) - 1) / Convert.ToInt32(RT[rtindex].Substring(0, 1));
                    tmpstr += (new string[] { " and ", "" }[Convert.ToInt32(IsSingleOrder)]) + "from " + d[dsrow[0].day - 1];
                    tmpstr += new string[] { " " + m[dsrow[0].Month - 1], "" }[Convert.ToInt32(dsrow[0].Month == dsrow[1].Month)];
                    tmpstr += " - " + d[dsrow[1].Day - 1] + " " + m[dsrow[1].Month - 1] + "(check out) for ";
                    tmpstr += cs[rcount - 1] + " ";
                    tmpstr += Regex.Match(RT[rtindex], new string[] { "(?<=,)[\\s\\S]+?(?=\\|)", "(?<=;)[\\s\\S]+?(?=,)" }[Convert.ToInt32(rcount <= 1)]).Groups[0].Value;
                    tmpstr += " at " + Convert.ToString(Convert.ToSingle(dsrow[4].ToString().Split(',')[0]) / rcount) + " CNY/Night" + new string[] { "", "/Person" }[Convert.ToInt32(rtindex < 3)];
                    IsSingleOrder = false;
                    */
                }
            }
            else
            {
                string SQL = "SELECT DISTINCT RoomType,Persons,Price FROM " + obj.Channel + " WHERE OrderNumber=\'" + obj.ResNumber + "\'";
                DataTable roomt = Select(SQL);
                foreach (DataRow dsrow in roomt.Rows)
                {
                    SQL = "SELECT Min(ReservedDate) As ArrivalDate,Max(ReservedDate) As DepartureDate FROM " + obj.Channel + " WHERE OrderNumber=\'" + obj.ResNumber + "\' And Persons=" + Convert.ToString(Convert.ToInt32(dsrow[1].ToString())) + " And Price=" + Convert.ToString(Convert.ToInt32(dsrow[2].ToString())) + " And RoomType=\'" + dsrow[0].ToString() + "\'";
                    DataTable datet = Select(SQL);
                    for (int i = 0; i <= RT.Length - 1; i++)
                    {
                        if (RT[i].IndexOf(dsrow[0].ToString()) < 0) continue;
                        rtindex = i;
                        break;
                    }
                    if (rtindex > 2)
                        rcount = 1;
                    else
                        rcount = Convert.ToInt32(dsrow[1]);
                    /*
                    tmpstr += new string[] { " and ", "" }[Convert.ToInt32(IsSingleOrder)] + "from " + d[datet.Rows[0][0].day - 1];
                    tmpstr += new string[] { " " + m[datet.Rows[0].Month - 1], "" }[Convert.ToInt32(datet.Rows[0][0].Month == datet.Rows[0][1].AddDays(1).Month)];
                    tmpstr += " - " + d[datet.Rows[0][1].AddDays(1).Day - 1] + " " + m[datet.Rows[0][1].AddDays(1).Month - 1] + "(check out) for ";
                    tmpstr += cs[new int[] { (int)dsrow[1] - 1, 0 }[Convert.ToInt32(rtindex > 2)]] + " ";
                    tmpstr += Regex.Match(RT[rtindex],
                        Convert.ToString(
                            new string[] { "(?<=,)[\\s\\S]+?(?=\\|)", "(?<=;)[\\s\\S]+?(?=,)" }
                            [Convert.ToInt32(rcount == 1)])).Groups[0].Value;
                    tmpstr += " at " + (int)dsrow[2] * (new int[] { 1, (int)dsrow[1] }[Convert.ToInt32(rtindex > 2)]) + " CNY/Night" + new string[] { "", "/Person" }[Convert.ToInt32(rtindex < 3)];
                    IsSingleOrder = false;
                    */
                }
            }
            return "";//_XmlReader.ReadValue("Email/EmailBody").Replace("RoomDetails", tmpstr).Replace("StaffName", staffnamecb.Text).Replace("CustomerName", tb_resDetails.Text.Substring(0, tb_resDetails.Text.IndexOf("\r\n")));
        }

        private bool EditDataBase(string SQL, string Prompt)
        {
            if (Run(SQL))
            {
                AppenErrorText(Prompt + "成功！" + "\r\n");
                return true;
            }
            else
            {
                AppenErrorText(Prompt + "失败！\r\n" + SQL + "\r\n");
                return false;
            }
        }

        private DateTime ConvertDateFromGrid(string Str)
        {
            return DateTime.ParseExact(Str,
                new string[] { "yyyy/MM/dd", "yyyy/M/dd", "yyyy/MM/d", "yyyy/M/d" },
                System.Globalization.DateTimeFormatInfo.InvariantInfo,
                System.Globalization.DateTimeStyles.None);
        }
        
        public void 设置SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(LocalPath + "Config.xml");
        }

        public void 刷新列表RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lb_order.Items.Clear();
            ResetRes();
            string SQL = string.Empty;
            SQL = "SELECT Channel,ResNumber FROM Main WHERE Checked=0 ORDER BY [ID]";
            DataTable dst = Select(SQL);
            if (dst.Rows.Count > 0)
            {
                for (int i = 0; i <= dst.Rows.Count - 1; i++)
                {
                    lb_order.Items.Add(new ListBoxResItem(dst.Rows[i][0].ToString(), dst.Rows[i][1].ToString()));
                }
            }
        }

        public void 更新配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _XmlReader = new EkiXmlDocument.EXmlReader(LocalPath, "Config");
            _XmlReader.Open();
            ResetRes();
        }
        
        #region SHARED

        private static bool IsNumeric(string str)
        {
            int a = 0;
            return Int32.TryParse(str, out a);
        }

        #endregion
        
        #region Res

        public void AddNewRes(ListBoxResItem res)
        {
            lb_order.Items.Add(res);
            lb_order.SelectedItem = res;
        }

        private void ResetRes()
        {
            tb_mainInfo.Text = string.Empty;
            //l_resDetails.Content = null;
            dg_order_room.ItemsSource = null;
            tb_order_error.Text = string.Empty;
            tb_order_content.Text = string.Empty;
        }

        public void MarkAllResChecked(bool isChecked)
        {
            Run("update MAIN set Checked=" + (isChecked ? "1" : "0"));
            lb_order.Items.Clear();
            ResetRes();
        }

        public void MarkResChecked(bool isChecked)
        {
            if (lb_order.SelectedItem == null)
            {
                MsgHelper.ShowMessage("必须选定一个订单！", "提示");
                return;
            }
            ListBoxResItem obj = lb_order.SelectedItem as ListBoxResItem;
            MarkResChecked(obj.ResNumber, isChecked);
            if (!isChecked)
                RemoveSelectedResItem();
        }

        public void FindOrder(string Keywords = "")
        {
            if (string.IsNullOrEmpty(Keywords))
                Keywords = Interaction.InputBox("请输入要查找的订单号或客人姓名：", "提示", "");
            if (string.IsNullOrEmpty(Keywords)) return;
            foreach (ListBoxResItem obj in lb_order.Items)
            {
                if (Keywords != obj.ResNumber) continue;
                Dispatcher.BeginInvoke((Action)delegate
                {
                    lb_order.SelectedItem = obj;
                    lb_order.ScrollIntoView(obj);
                });
                return;
            }
            string SQL = "SELECT WebSite,OrderNumber FROM Main WHERE " +
                "OrderNumber LIKE \'%" + Keywords + "%\' OR " +
                "CustomerName LIKE \'%" + Keywords + "%\'";
            DataTable dst = Select(SQL);
            if (dst.Rows.Count > 0)
            {
                foreach (DataRow row in dst.Rows)
                {
                    ListBoxResItem obj = new ListBoxResItem(row[0].ToString(), row[1].ToString());
                    UpdateRes(obj.ResNumber, "Checked", "0");
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        lb_order.Items.Add(obj);
                        lb_order.SelectedItem = obj;
                        lb_order.ScrollIntoView(obj);
                    });
                }
                AppenErrorText("已找到关于\"" + Keywords + "\"的历史订单！" + "\r\n");
            }
            else
            {
                AppenErrorText("未找到关于\"" + Keywords + "\"的历史订单！" + "\r\n");
            }
        }
        
        public string EmailText(string staffName)
        {
            ListBoxResItem res = lb_order.SelectedItem as ListBoxResItem;
            if (res == null) return null;
            return _EmailHelper.BuildRoomDetails(_Conn, _XmlReader, res.Channel, res.ResNumber,
                staffName, res.GuestName);
        }

        #endregion

        #region EmailTemplet

        public List<string> ReadEmailTempletList()
        {
            string path = Environment.CurrentDirectory;
            if (!path.EndsWith("\\")) path += "\\";
            path += "EmailTemplets";
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
            string path = Environment.CurrentDirectory;
            if (!path.EndsWith("\\")) path += "\\";
            path += "EmailTemplets\\" + mi.Label.ToString() + ".txt";
            FileStream fs = new FileStream(path, FileMode.Open);
            Clipboard.SetText(new StreamReader(fs).ReadToEnd());
            fs.Close();
            MsgHelper.ShowMessage("复制邮件模板成功！", mi.Parent as UIElement);
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
            ListBoxResItem obj = lb_order.SelectedItem as ListBoxResItem;
            if (obj == null) return;
            ResetRes();
            #region DATAGRID

            string sqlString = "SELECT " + _XmlReader.ReadValue(obj.Channel + "/RoomKeywords").Replace("OrderNumber,", "") + " FROM " + obj.Channel + " WHERE OrderNumber=\'" + obj.ResNumber + "\'";
            DataTable dt = Select(sqlString);
            if (dt.Rows.Count < 1)
            {
                AppenErrorText("读取房间数据失败！请检查是否有订单号为：" + obj.ResNumber + "的预订房型记录。" + "\r\n");
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

            #endregion
            #region MAININFO

            sqlString = "SELECT " + _XmlReader.ReadValue("Main/ShowKeywords") +
                " FROM Main WHERE OrderNumber='" + obj.ResNumber + "'";
            DataTable dst = Select(sqlString);
            if (dst.Rows.Count < 1) return;
            string tmp = "";
            foreach (string title in new string[] { "CustomerName",
                    "OrderNumber", "EmailAddress" })
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
                    contentString += "\r\n" + disnames[i] + ": " + ReadPriceString(obj);
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

        private void RemoveSelectedResItem()
        {
            int resIndex = lb_order.SelectedIndex;
            lb_order.Items.RemoveAt(resIndex);
            if (resIndex > 0) resIndex--;
            if (lb_order.Items.Count > 0)
                lb_order.SelectedIndex = resIndex;
            else
                ResetRes();
        }

        public void DeleteSelectedResItem()
        {
            if ((lb_order.SelectedItem == null)) return;
            ListBoxResItem obj = lb_order.SelectedItem as ListBoxResItem;
            DeleteResByNumber(obj);
            lb_order.Items.Remove(obj);
        }
        
        #endregion

        #region CONNECTION

        private DataTable Select(string sql)
        {
            if (sql == null || sql == "") return null;
            string tableName = "反正是个没有人看得到的彩蛋，那我写得长一点也没关系吧";
            OleDbDataAdapter da = new OleDbDataAdapter(sql, _Conn);
            DataSet ds = new DataSet();
            da.Fill(ds, tableName);
            return ds.Tables[tableName];
        }

        private bool ExistRes(string resNumber)
        {
            _Comm.CommandText = "select * from Main where OrderNumber='" + resNumber + "'";
            return _Comm.ExecuteScalar() != null;
        }

        private bool Run(string sql)
        {
            _Comm.CommandText = sql;
            try
            {
                _Comm.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string ReadPriceString(ListBoxResItem res)
        {
            string sqlString = "select price from " + res.Channel + 
                " where OrderNumber='" + res.ResNumber + "'";
            DataTable dt = Select(sqlString);
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

        private void DeleteResByNumber(ListBoxResItem res)
        {
            Run("delete from Main where OrderNumber='" + res.ResNumber + "'");
            Run("delete from " + res.Channel + " where OrderNumber='" + res.ResNumber + "'");
        }

        private void UpdateRes(string resNumber, string key, string value)
        {
            Run("update Main set " + key + "=" + value + " where OrderNumber='" + resNumber + "'");
        }

        private void MarkResChecked(string resNumber, bool isChecked)
        {
            UpdateRes(resNumber, "Checked", (isChecked ? "1" : "0"));
        }

        private string ItemToString(object obj, bool isForGrid)
        {
            if (obj == null) return string.Empty;
            string result = string.Empty;
            if (obj.GetType() == typeof(DateTime))
                if (isForGrid)
                    result = ((DateTime)obj).ToString("yyyy年MM月dd日");
                else
                    result = ((DateTime)obj).ToString("yyyy/MM/dd");
            else
                result = obj.ToString();
            return result;
        }

        #endregion

    }
}
