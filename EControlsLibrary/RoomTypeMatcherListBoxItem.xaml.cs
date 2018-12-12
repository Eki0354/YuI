using ELite.ELiteItem;
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

namespace EControlsLibrary
{
    /// <summary>
    /// RoomTypeMatcherListBoxItem.xaml 的交互逻辑
    /// </summary>
    public partial class RoomTypeMatcherListBoxItem : UserControl
    {
        private ELiteRoomTypeMatchItem _MatchItem;
        public ELiteRoomTypeMatchItem MatchItem => _MatchItem;
        public bool IsChanged { get; private set; } = false;

        public RoomTypeMatcherListBoxItem()
        {
            InitializeComponent();
        }

        private void ComboBoxX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ELiteRoomTypeItem type = (ELiteRoomTypeItem)(ComboBoxX.SelectedItem);
            _MatchItem.RTID = type.RTID;
            IsChanged = true;
        }

        private void TextBoxX_TextChanged(object sender, TextChangedEventArgs e)
        {
            _MatchItem.MatchChar = TextBoxX.Text;
            IsChanged = true;
        }

        public void Initialize(string matchChar, List<ELiteRoomTypeItem> types)
        {
            _MatchItem = ELiteRoomTypeMatchItem.Empty;
            _MatchItem.MatchChar = matchChar;
            TextBoxX.Text = MatchItem.MatchChar;
            ComboBoxX.ItemsSource = types;
            TextBoxX.TextChanged += TextBoxX_TextChanged;
            ComboBoxX.SelectionChanged += ComboBoxX_SelectionChanged;
        }

        public void Initialize(ELiteRoomTypeMatchItem item, List<ELiteRoomTypeItem> types)
        {
            _MatchItem = item;
            TextBoxX.Text = _MatchItem.MatchChar;
            ComboBoxX.ItemsSource = types;
            ComboBoxX.SelectedIndex = types.FindIndex(type => type.RTID == _MatchItem.RTID);
            TextBoxX.TextChanged += TextBoxX_TextChanged;
            ComboBoxX.SelectionChanged += ComboBoxX_SelectionChanged;
        }
    }
}
