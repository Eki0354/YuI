using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SQLite;
using System.Windows.Controls.Ribbon;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using ELite;
using EkiXmlDocument;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using ControlItemCollection;
using Reservation;

namespace Interface_Reception_Ribbon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        #region PROPERTY

        public static ELiteConnection _Conn;
        RibbonTab _SelectedRibbonTab = null;

        #endregion

        #region PAGES_REGISTER

        Page_Sale _pageSale = new Page_Sale();
        Page_Room _pageRoom = new Page_Room();
        Page_Reservation _pageRes = new Page_Reservation();

        #endregion

        #region Definitions

        /// <summary> 剪贴板内容改变时API函数向windows发送的消息 </summary>
        const int WM_CLIPBOARDUPDATE = 0x031D;

        /// <summary> windows用于监视剪贴板的API函数 </summary>
        /// <param name="hwnd">要监视剪贴板的窗口的句柄</param>
        /// <returns>成功则返回true</returns>
        [DllImport("user32.dll")]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        /// <summary> 取消对剪贴板的监视 </summary>
        /// <param name="hwnd">监视剪贴板的窗口的句柄</param>
        /// <returns>成功则返回true</returns>
        [DllImport("user32.dll")]
        public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        #endregion
        
        #region CLIPBOARD

        /// <summary> WPF窗口重写 </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            //添加监视消息事件
            if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            // HTodo  ：添加剪贴板监视 
            IntPtr handle = (new WindowInteropHelper(this)).Handle;
            AddClipboardFormatListener(handle);
        }

        /// <summary> 剪贴板监视事件 </summary>
        protected IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_CLIPBOARDUPDATE:
                    OnClipboardChanged();
                    break;
            }
            return IntPtr.Zero;
        }

        /// <summary> 剪贴板内容改变具体响应，根据需求，程序仅对文字内容进行监视 </summary>
        void OnClipboardChanged()
        {
            string text = Clipboard.GetText();
            if (string.IsNullOrEmpty(text)) return;
            ListBoxResItem res = Getter.GetReservation(_Conn, text);
            if (res == null) return;
            //_pageRes.ResListBox.Items.Add(res);    //添加新订单
            //_pageRes.ResListBox.SelectedItem = res;//自动选中新添加的订单项
            Clipboard.SetText(string.Empty);
        }

        #endregion

        #region MAIN

        static List<Dictionary<string, string>> order_items = new List<Dictionary<string, string>>();
        static List<DataTable> order_rooms = new List<DataTable>();
        public MainWindow()
        {
            InitializeComponent();
            InitializeControls();
            _Conn = new ELiteConnection("", "eki", "db");
            _Conn.Open();
            _pageRes.InitializeResListBox(_Conn.UncheckedResList());
        }

        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_Conn != null)
                _Conn.Close();
            _Conn = null;
        }

        private void InitializeControls()
        {
            InitializeRoomControls();
            //InitializeControls_ResGroup();
        }

        private void MainRibbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainRibbon.SelectedItem.GetType() != typeof(RibbonTab)) { return; }
            RibbonTab rt = mainRibbon.SelectedItem as RibbonTab;
            if (_SelectedRibbonTab == rt) return; 
            _SelectedRibbonTab = rt;
            switch (_SelectedRibbonTab.Header)
            {
                case "房态":
                    mainFrame.Content = _pageRoom;
                    break;
                case "订单管理":
                    mainFrame.Content = _pageRes;
                    break;
                case "销售":
                    mainFrame.Content = _pageSale;
                    break;
            }
        }

        #endregion

        #region QUICKBAR

        #endregion

        #region MENU

        private void menu_options_Click(object sender, RoutedEventArgs e)
        {
            
        }

        #endregion

        #region TABS

        #region ROOMSTATUS

        private void button_test_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void InitializeRoomControls()
        {
            //初始化Room界面入住/退房日期，默认为当前日期前后五天
            DateTime today = DateTime.Now;
            dp_room_checkin.SelectedDate = today.AddDays(-15);
            dp_room_checkout.SelectedDate = today.AddDays(15);
        }

        private void selectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_pageRoom == null || dp_room_checkin == null || dp_room_checkout == null)
                return;
            if (dp_room_checkout.SelectedDate < dp_room_checkin.SelectedDate)
                dp_room_checkout.SelectedDate = dp_room_checkin.SelectedDate;
            _pageRoom.ChangeDisplayDate((DateTime)dp_room_checkin.SelectedDate, (DateTime)dp_room_checkout.SelectedDate);
        }

        #endregion

        #region SALE

        private void rb_sale_addItem_Click(object sender, RoutedEventArgs e)
        {
            _pageSale.ShowPopup_AddItem(true);
        }



        #endregion

        #region RESERVATION

        private void InitializeControls_ResGroup()
        {
            return;
        }

        #endregion

        #endregion
        
    }
}
