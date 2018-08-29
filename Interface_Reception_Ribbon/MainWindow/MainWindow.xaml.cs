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
using ELite.Reservation;

namespace Interface_Reception_Ribbon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        //public static ELiteConnection _Conn;
        RibbonTab _SelectedRibbonTab = null;
        //ResAssist _ResAssist;
        //ResGetter _ResGetter;
        
        static List<Dictionary<string, string>> order_items = new List<Dictionary<string, string>>();
        static List<DataTable> order_rooms = new List<DataTable>();
        public MainWindow()
        {
            InitializeComponent();
            //_Conn = new ELiteConnection("", "eki", "db");
            //_Conn.Open();
            _pageSale = new Page_Sale();
            _pageRoom = new Page_Room();
            _pageRes = new Page_Reservation();
            MainRibbon_SelectionChanged(null, null);
            //_ResAssist = new ResAssist(_Conn);
            //_ResGetter = new ResGetter(_Conn);
            InitializeControls();
        }

        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            /*if (_Conn != null)
                _Conn.Close();
            _Conn = null;*/
        }

        private void InitializeControls()
        {
            InitializeControls_Room();
            InitializeControls_ResGroup();
        }

        private void MainRibbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainRibbon.SelectedItem.GetType() != typeof(RibbonTab)) { return; }
            RibbonTab rt = mainRibbon.SelectedItem as RibbonTab;
            if (mainFrame.Content != null && _SelectedRibbonTab == rt) return;
            _SelectedRibbonTab = rt;
            switch (_SelectedRibbonTab.Header.ToString())
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

        private void RibbonWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (_SelectedRibbonTab == null) return;
            if (_SelectedRibbonTab.Header.ToString() == "订单管理")
            {
                switch(e.Key)
                {
                    case Key.F1:
                        CopyEmailReplies("0");
                        break;
                    case Key.F2:
                        CopyEmailReplies("1");
                        break;
                    case Key.F3:
                        CopyEmailReplies("2");
                        break;
                }
                if((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || 
                    e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) &&
                    e.KeyboardDevice.IsKeyDown(Key.F))
                {
                    _pageRes.FindOrder();
                }
                if ((e.KeyboardDevice.IsKeyDown(Key.LeftShift) || 
                    e.KeyboardDevice.IsKeyDown(Key.RightShift)) && 
                    e.KeyboardDevice.IsKeyDown(Key.Delete))
                {
                    _pageRes.DeleteSelectedResItem();
                }
            }
        }
    }
}
