using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Interface_Reception_Ribbon
{
    public partial class MainWindow
    {
        Page_Sale _pageSale;

        private void rb_sale_addItem_Click(object sender, RoutedEventArgs e)
        {
            _pageSale.ShowPopup_AddItem(true);
        }
    }
}
