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
using ELite;
using System.Globalization;
using System.Windows.Controls.Primitives;

namespace Interface_Reception_Ribbon
{
    /// <summary>
    /// Page_Room.xaml 的交互逻辑
    /// </summary>
    public partial class Page_Room : Page
    {
        ELiteConnection _Conn = MainWindow.Conn;

        public Page_Room()
        {
            InitializeComponent();
        }

        #region DATAGRID

        DateTime _StartDate;
        DateTime _EndDate;
        DataTable _CurrentRoomSet = new DataTable();

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
            _CurrentRoomSet = _Conn.GetGridRoomItemSet(date_from, date_to);
            dg_room.ItemsSource = _CurrentRoomSet.DefaultView;
        }

        #endregion

        private void MonthRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = e.Source as RadioButton;
            if (rb == null) return;
            ComboBoxItem yearItem = cb_year.SelectedItem as ComboBoxItem;
            if (yearItem == null) return;
            string yearText = yearItem.Content.ToString();
            if (string.IsNullOrEmpty(yearText)) return;
            int year = Convert.ToInt32(yearText);
            int month = Convert.ToInt32(rb.Content.ToString().Substring(0, 2));
            _StartDate = new DateTime(year, month, 1);
            _EndDate= new DateTime(year, month, DateTime.DaysInMonth(year, month));
            UpdateSelectedRoomDataTable(_StartDate, _EndDate);
        }
    }

    public class CellCoord
    {
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public CellCoord(int rowIndex, int columnIndex)
        {
            RowIndex = RowIndex;
            ColumnIndex = columnIndex;
        }
    }
}
