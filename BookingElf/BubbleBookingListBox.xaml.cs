using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BookingElf
{
    /// <summary>
    /// RoomTypeListBox.xaml 的交互逻辑
    /// </summary>
    public partial class BubbleBookingListBox : ListBox
    {
        #region EventHandler

        public class ItemsRemovedEventArgs : RoutedEventArgs
        {
            public List<BubbleBookingItem> RemovedBubbles { get; internal set; }
        }

        ItemsRemovedEventArgs SelectionRemovedEventArgs = new ItemsRemovedEventArgs();

        ItemsRemovedEventArgs ItemsClearedEventArgs => new ItemsRemovedEventArgs();

        public delegate void DeleteMenuItemClickHandle(object sender, ItemsRemovedEventArgs e);

        public event DeleteMenuItemClickHandle DeleteMenuItemClicked;

        #endregion

        #region DependencyProperty
        
        public BubbleBookingItemCollection Bubbles
        {
            get { return (BubbleBookingItemCollection)GetValue(BubblesProperty); }
            set { SetValue(BubblesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Bubbles.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BubblesProperty =
            DependencyProperty.Register("Bubbles",
                typeof(BubbleBookingItemCollection),
                typeof(BubbleBookingListBox),
                new PropertyMetadata(new BubbleBookingItemCollection()));
        
        public List<string> Channels
        {
            get { return (List<string>)GetValue(ChannelsProperty); }
            set { SetValue(ChannelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Channels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChannelsProperty =
            DependencyProperty.Register("Channels",
                typeof(List<string>),
                typeof(BubbleBookingListBox),
                new PropertyMetadata(new List<string>()));

        #endregion
        
        public BubbleBookingListBox()
        {
            InitializeComponent();
            Channels.Add("Booking");
            Channels.Add("HostelWorld");
        }
        
        private void RemoveSelections()
        {
            foreach(var item in this.SelectedItems)
            {
                if (item is BubbleBookingItem bubble)
                    Bubbles[this.Items.IndexOf(item)].IsDeleteMarked = true;
            }
            SelectionRemovedEventArgs.RemovedBubbles = Bubbles.DeleteMarkedItems;
            Bubbles.RemoveDeleteMarkedItems();
        }

        #region MenuEvents

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelections();
            DeleteMenuItemClicked?.Invoke(sender, SelectionRemovedEventArgs);
        }

        #endregion

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectedItem is BubbleBookingItem item) this.ScrollIntoView(item);
        }

        public int FindByResNumber(string resNumber)
        {
            int index = Bubbles.IndexOfResNumber(resNumber);
            if (index < 0) return -1;
            Bubbles[index] = Bubbles[index].ToSearchResult();
            this.SelectedIndex = index;
            return index;
        }
    }
}
