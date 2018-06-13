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
using System.Windows.Controls.Ribbon;
using System.Windows.Threading;
using System.Text.RegularExpressions;

namespace Interface_Reception_Ribbon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        #region PROPERTY

        string _selectedTabName = "";

        #endregion

        #region PAGES_REGISTER

        Page_Sale _pageSale = new Page_Sale();
        Page_Room _pageRoom = new Page_Room();
        Page_Reservation _pageRes = new Page_Reservation();

        #endregion

        static List<Dictionary<string, string>> order_items = new List<Dictionary<string, string>>();
        static List<DataTable> order_rooms = new List<DataTable>();
        public MainWindow()
        {
            InitializeComponent();
            InitializeControls();
            //在窗口大小改变时更改房态Grid的大小
            this.SizeChanged += new SizeChangedEventHandler(MainWindow_Resize);
        }
        private void InitializeOrderData()
        {
            //DataTable dt = _conn.Select("select Order,Channel from info_res where Checked=false");
            //lb_order.ItemsSource = dt.DefaultView;
        }
        private void MainWindow_Resize(object sender, EventArgs e)
        {
            //动态改变datagrid最大宽度
            //dg_room.MaxWidth = this.ActualWidth - (rg_info.Visibility == Visibility.Visible ? 200 : 0) -
                //(rg_choose.Visibility == Visibility.Visible ? 210 : 0) - 15;
        }
        private void InitializeControls()
        {
            InitializeRoomControls();
        }
        

        #region MAIN

        private void MainRibbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainRibbon.SelectedItem.GetType() != typeof(RibbonTab)) { return; }
            RibbonTab rt = mainRibbon.SelectedItem as RibbonTab;
            if (_selectedTabName == rt.Name) { return; }
            _selectedTabName = rt.Name;
            switch (_selectedTabName)
            {
                case "tab_room":
                    mainFrame.Content = _pageRoom;
                    break;
                case "tab_web":
                    mainFrame.Content = null;
                    break;
                case "tab_order":
                    mainFrame.Content = _pageRes;
                    break;
                case "tab_sale":
                    mainFrame.Content = _pageSale;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region ROOM

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

        
    }
}
