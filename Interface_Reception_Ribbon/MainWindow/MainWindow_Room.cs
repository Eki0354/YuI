using EControlsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic;
using ELite.Reservation;

namespace Interface_Reception_Ribbon
{
    public partial class MainWindow
    {
        Page_Room _pageRoom;

        private void button_test_Click(object sender, RoutedEventArgs e)
        {
            _pageRoom.rtlb_res.Items.Add(ELiteListBoxResItem.Empty);
            return;
            string year = Interaction.InputBox("年份", "", "2018");
            string path = Interaction.InputBox("Excel文件路径", "");
            _Conn.TransferFromExcel(Int32.Parse(year), path);
        }

        private void InitializeControls_Room()
        {
            //初始化Room界面入住/退房日期，默认为当前日期前后五天
            DateTime today = DateTime.Now;
            //dp_room_from.SelectedDate = today.AddDays(-15);
            //dp_room_to.SelectedDate = today.AddDays(15);
        }

        private void selectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_pageRoom == null || dp_room_from == null || dp_room_to == null)
                return;
            if (dp_room_to.SelectedDate < dp_room_from.SelectedDate)
                dp_room_to.SelectedDate = dp_room_from.SelectedDate;
            _pageRoom.UpdateDisplayDate((DateTime)dp_room_from.SelectedDate, (DateTime)dp_room_to.SelectedDate);
        }

        private void button_editRoom_Click(object sender, RoutedEventArgs e)
        {
            _Conn.AutoArrangeRes();
        }
        
        private void MatchRoomType(object sender, RoutedEventArgs e)
        {
            RoomTypeMatcher.ShowRoomTypeMatcher(_Conn);
        }
    }
}
