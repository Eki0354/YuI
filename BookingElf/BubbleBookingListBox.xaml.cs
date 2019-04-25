using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

        public class BubblesChangedEventArgs : RoutedEventArgs
        {
            public List<BubbleBookingItem> ChangedBubbles { get; internal set; }
        }

        BubblesChangedEventArgs SelectionsChangedEventArgs = new BubblesChangedEventArgs();

        BubblesChangedEventArgs AllChangedEventArgs = new BubblesChangedEventArgs();

        public delegate void DeleteMenuItemClickHandle(object sender, BubblesChangedEventArgs e);

        public event DeleteMenuItemClickHandle DeleteMenuItemClicked;

        public delegate void HideSelectionsMenuItemClickHandle(object sender, BubblesChangedEventArgs e);

        public event HideSelectionsMenuItemClickHandle HideSelectionsMenuItemClicked;

        public delegate void HideAllItemsMenuItemClickHandle(object sender, BubblesChangedEventArgs e);

        public event HideAllItemsMenuItemClickHandle HideAllItemsMenuItemClicked;

        public delegate void MarkSelectionsMenuItemClickHandle(object sender, BubblesChangedEventArgs e);

        public event MarkSelectionsMenuItemClickHandle MarkSelectionsMenuItemClicked;

        public delegate void MarkAllItemsMenuItemClickHandle(object sender, BubblesChangedEventArgs e);

        public event MarkAllItemsMenuItemClickHandle MarkAllItemsMenuItemClicked;

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
        
        private void ListBox_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            if (!(this.ItemsSource is BubbleBookingItemCollection)) this.ItemsSource = null;
        }

        private void ListBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(this.DataContext is BubbleBookingItemCollection)) this.DataContext = null;
        }

        private void RemoveSelections()
        {
            GetMarkedSelections().ForEach(bubble => Bubbles.Remove(bubble));
        }

        private void ClearItems()
        {
            GetMarkedAllItems();
            Bubbles.Clear();
        }

        private List<BubbleBookingItem> GetMarkedSelections()
        {
            List<BubbleBookingItem> bubbleList = new List<BubbleBookingItem>();
            foreach (var item in this.SelectedItems)
            {
                bubbleList.Add(Bubbles[this.Items.IndexOf(item)]);
            }
            SelectionsChangedEventArgs.ChangedBubbles = bubbleList;
            return bubbleList;
        }

        private List<BubbleBookingItem> GetMarkedAllItems()
        {
            List<BubbleBookingItem> items = new List<BubbleBookingItem>(Bubbles);
            AllChangedEventArgs.ChangedBubbles = items;
            return items;
        }

        private void MarkItems(List<BubbleBookingItem> items, string header)
        {
            items.ForEach(item =>
            {
                switch (header)
                {
                    case "修改":
                        item.State = BubbleBookingState.Changed;
                        break;
                    case "取消":
                        item.State = BubbleBookingState.Cancelled;
                        break;
                    case "无效":
                        item.State = BubbleBookingState.Invalid;
                        break;
                    case "正常":
                    default:
                        item.State = BubbleBookingState.Normal;
                        break;
                }
            });
        }

        #region MenuEvents

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelections();
            DeleteMenuItemClicked?.Invoke(sender, SelectionsChangedEventArgs);
        }
        
        private void MenuHideSelections_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelections();
            HideSelectionsMenuItemClicked?.Invoke(sender, SelectionsChangedEventArgs);
        }

        private void MenuHideAllItems_Click(object sender, RoutedEventArgs e)
        {
            ClearItems();
            HideAllItemsMenuItemClicked?.Invoke(sender, AllChangedEventArgs);
        }

        private void MenuMarkSelections_Click(object sender, RoutedEventArgs e)
        {
            MarkItems(GetMarkedSelections(), (sender as MenuItem).Header.ToString());
            MarkSelectionsMenuItemClicked?.Invoke(sender, SelectionsChangedEventArgs);
        }

        private void MenuMarkAllItems_Click(object sender, RoutedEventArgs e)
        {
            MarkItems(GetMarkedAllItems(), (sender as MenuItem).Header.ToString());
            MarkAllItemsMenuItemClicked?.Invoke(sender, AllChangedEventArgs);
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
