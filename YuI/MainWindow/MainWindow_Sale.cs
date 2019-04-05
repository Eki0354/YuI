using System.Windows;

namespace YuI
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
