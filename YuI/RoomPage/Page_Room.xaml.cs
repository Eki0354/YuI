using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SQLite;
using MementoConnection.ELiteItem;
using MMC = MementoConnection.MMConnection;

namespace YuI
{
    /// <summary>
    /// Page_Room.xaml 的交互逻辑
    /// </summary>
    public partial class Page_Room : Page
    {
        public Page_Room()
        {
            InitializeComponent();
        }

        #region DATAGRID

        DateTime _StartDate;
        DateTime _EndDate;
        ELiteResRoomItemCollection ResRoomItems { get; set; }

        public void UpdateDisplayDate(DateTime date_from, DateTime date_to)
        {
            if (_StartDate == date_from && _EndDate == date_to) return;
            if (date_to < date_from || (date_to - date_from).Days > 31) return;
            _StartDate = date_from;
            _EndDate = date_to;
            UpdateSelectedRoomDataTable(_StartDate, _EndDate);
        }

        private void UpdateSelectedRoomDataTable(DateTime date_from, DateTime date_to)
        {
            return;
            ResRoomItems = new ELiteResRoomItemCollection(date_from, date_to);
            ResRoomItems.InitVisualItemSet();
            this.dg_room.ItemsSource = null;
            this.lb_room.ItemsSource = null;
            this.dg_room.ItemsSource = ResRoomItems.VisualItemSet.DefaultView;
            this.lb_room.ItemsSource = ResRoomItems.UnscheduledResRoomItems;
        }

        #endregion

        private void MonthRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = e.Source as RadioButton;
            if (rb == null) return;
            int year = ymSelector.Year;
            int month = ymSelector.Month;
            _StartDate = new DateTime(year, month, 1);
            _EndDate= new DateTime(year, month, DateTime.DaysInMonth(year, month));
            UpdateSelectedRoomDataTable(_StartDate, _EndDate);
        }
    }
}
