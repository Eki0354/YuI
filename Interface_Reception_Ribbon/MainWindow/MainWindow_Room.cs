using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Interface_Reception_Ribbon
{
    public partial class MainWindow
    {
        Page_Room _pageRoom;

        private void button_test_Click(object sender, RoutedEventArgs e)
        {

        }

        private void InitializeControls_Room()
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
    }
}
