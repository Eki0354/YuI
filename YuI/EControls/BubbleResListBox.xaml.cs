using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Interface_Reception_Ribbon.EControls
{
    /// <summary>
    /// RoomTypeListBox.xaml 的交互逻辑
    /// </summary>
    public partial class BubbleResListBox : ListBox
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
                typeof(double), typeof(BubbleResListBox),
                new PropertyMetadata(18.0));
        
        public double NameFontSize
        {
            get { return (double)GetValue(NameFontSizeProperty); }
            set { SetValue(NameFontSizeProperty, value); }
        }

        public static readonly DependencyProperty NameFontSizeProperty =
            DependencyProperty.Register("NameFontSizeProperty",
                typeof(double), 
                typeof(BubbleResListBox),
                new PropertyMetadata(12.0));
        
        public BubbleResCollection Bubbles
        {
            get { return (BubbleResCollection)GetValue(BubblesProperty); }
            set { SetValue(BubblesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Bubbles.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BubblesProperty =
            DependencyProperty.Register("Bubbles",
                typeof(BubbleResCollection),
                typeof(BubbleResListBox),
                new PropertyMetadata(new BubbleResCollection()));
        
        public List<string> Channels
        {
            get { return (List<string>)GetValue(ChannelsProperty); }
            set { SetValue(ChannelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Channels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChannelsProperty =
            DependencyProperty.Register("Channels",
                typeof(List<string>),
                typeof(BubbleResListBox),
                new PropertyMetadata(new List<string>()));

        #endregion
        
        public BubbleResListBox()
        {
            InitializeComponent();
            Channels.Add("Booking");
            Channels.Add("HostelWorld");
        }

        private void RemoveSelectedItems()
        {
            IList items = this.SelectedItems;
            for (int i = items.Count - 1; i > -1; i--)
            {
                Bubbles.Remove(items[i] as BubbleResListBoxItem);
            }
        }

        #region MenuEvents

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelectedItems();
            DeleteMenuItemClicked?.Invoke(sender, new RoutedEventArgs());
        }

        #endregion

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectedItem is null) return;
            this.ScrollIntoView(this.SelectedItem);
        }
    }
}
