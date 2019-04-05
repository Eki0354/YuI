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

        public delegate void DeleteMenuItemClickHandle(object sender, RoutedEventArgs e);

        public event DeleteMenuItemClickHandle DeleteMenuItemClicked;

        #endregion

        #region DependencyProperty

        public double ChannelFontSize
        {
            get { return (double)GetValue(ChannelFontSizeProperty); }
            set { SetValue(ChannelFontSizeProperty, value); }
        }

        public static readonly DependencyProperty ChannelFontSizeProperty =
            DependencyProperty.Register("ChannelFontSizeProperty",
                typeof(double), typeof(BubbleBookingListBox),
                new PropertyMetadata(18.0));
        
        public double NameFontSize
        {
            get { return (double)GetValue(NameFontSizeProperty); }
            set { SetValue(NameFontSizeProperty, value); }
        }

        public static readonly DependencyProperty NameFontSizeProperty =
            DependencyProperty.Register("NameFontSizeProperty",
                typeof(double), 
                typeof(BubbleBookingListBox),
                new PropertyMetadata(12.0));
        
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
        
        #region MenuEvents

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DeleteMenuItemClicked?.Invoke(sender, new RoutedEventArgs());
        }

        #endregion

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectedItem is BubbleBookingItem item) this.ScrollIntoView(item);
        }
    }
}
