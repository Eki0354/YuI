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

namespace BookingElf
{
    public class BubbleListbox : ListBox
    {
        public BubbleListbox()
        {

        }

        static BubbleListbox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BubbleListbox), new FrameworkPropertyMetadata(typeof(BubbleListbox)));
        }

        #region EventHandler

        public delegate void DeleteMenuItemClickHandle(object sender, RoutedEventArgs e);

        public event DeleteMenuItemClickHandle DeleteMenuItemClicked;

        #endregion

        #region DependencyProperty
        
        public BubbleBookingItemCollection Bubbles
        {
            get { return (BubbleBookingItemCollection)GetValue(BubblesProperty); }
            set { SetValue(BubblesProperty, value); }
        }

        public static readonly DependencyProperty BubblesProperty =
            DependencyProperty.Register("Bubbles",
                typeof(BubbleBookingItemCollection),
                typeof(BubbleListbox),
                new PropertyMetadata(new BubbleBookingItemCollection()));
        
        /*private PropertyChangedCallback BubblesChangedCallback()
        {

        }*/

        #endregion

        #region MenuEvents

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //RemoveSelectedItems();
            DeleteMenuItemClicked?.Invoke(sender, new RoutedEventArgs());
        }

        #endregion
        

        private void BubbleListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectedItem is null) return;
            this.ScrollIntoView(this.SelectedItem);
        }

    }
}
