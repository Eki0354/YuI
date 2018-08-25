using System;
using System.Collections.Generic;
using System.Windows.Controls;
using ELite.Reservation;
using System.Data;

namespace Interface_Reception_Ribbon
{
    public partial class Page_Reservation : Page
    {
        #region PROPERTY
        
        private string _LocalPath = Environment.CurrentDirectory;
        //public ListBox ResListBox { get { return lb_order; } }
        private ListBoxResItem _CurrentRes;

        #endregion

        public Page_Reservation()
        {
            InitializeComponent();
        }

        public void InitializeResListBox(List<ListBoxResItem> resList)
        {
            ResetOrder();
            lb_order.Items.Clear();
            resList.ForEach(res => lb_order.Items.Add(res));
        }

        #region SHARED

        private static bool IsNumeric(string str)
        {
            int a = 0;
            return Int32.TryParse(str, out a);
        }

        #endregion

        #region PUBLIC

        public void AddNewRes(ListBoxResItem res)
        {
            lb_order.Items.Add(res);
            lb_order.SelectedItem = res;
        }

        #endregion

        private void ResetOrder()
        {
            tb_mainInfo.Text = string.Empty;
            //l_resDetails.Content = null;
            dg_order_room.ItemsSource = null;
            label_order_error.Text = string.Empty;
            label_order_content.Text = string.Empty;
        }

        private void lb_order_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object obj = ((ListBox)sender).SelectedItem;
            if (obj == _CurrentRes) return;
            ResetOrder();
            _CurrentRes = obj as ListBoxResItem;
            DataTable roomDT = new DataTable();
            Booking booking = MainWindow._Conn.GetBooking(_CurrentRes, out roomDT);
            dg_order_room.ItemsSource = roomDT.DefaultView;
            tb_mainInfo.Text = booking.ToMainInfoString();
            //l_resDetails.Content = booking.ToDetailsString();
        }
    }
}
