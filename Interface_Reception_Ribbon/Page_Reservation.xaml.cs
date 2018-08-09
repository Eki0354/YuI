using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Forms;
using ControlItemCollection;
using Mrs_Panda预订快捷工具;

namespace Interface_Reception_Ribbon
{
    public partial class Page_Reservation : Page
    {
        #region PROPERTY

        private mainform form = new mainform();
        private string _LocalPath = Environment.CurrentDirectory;
        //public ListBox ResListBox { get { return lb_order; } }

        #endregion

        public Page_Reservation()
        {
            InitializeComponent();
            form.FormBorderStyle = FormBorderStyle.None; // 无边框
            form.TopLevel = false; // 不是最顶层窗体
            formPanel.Controls.Add(form);  // 添加到 Panel中
            form.Show();
        }

        public void InitializeResListBox(List<ListBoxResItem> resList)
        {
            //lb_order.Items.Clear();
            //resList.ForEach(res => lb_order.Items.Add(res));
        }

        #region SHARED

        private static bool IsNumeric(string str)
        {
            int a = 0;
            return Int32.TryParse(str, out a);
        }

        #endregion

        private void lb_order_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ListBoxResItem res = lb_order.SelectedItem as ListBoxResItem;
            //dg_order_room.ItemsSource = MainWindow._Conn.GetResRooms(res.ResNumber).DefaultView;
        }
    }
}
