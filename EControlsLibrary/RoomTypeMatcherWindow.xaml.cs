using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
using System.Windows.Shapes;
using ELite;
using ELite.ELiteItem;

namespace EControlsLibrary
{
    /// <summary>
    /// RoomTypeMatcher.xaml 的交互逻辑
    /// </summary>
    public partial class RoomTypeMatcherWindow : Window
    {
        ELiteConnection _Conn;

        public RoomTypeMatcherWindow()
        {
            InitializeComponent();
        }

        public void Initialize(ELiteConnection conn)
        {
            if (conn == null) return;
            _Conn = conn;
            rb_unmatched.IsChecked = true;
        }

        private void LoadItems(List<string> matchChars, List<ELiteRoomTypeItem> types)
        {
            ListBoxX.Items.Clear();
            matchChars.ForEach(mChar =>
            {
                RoomTypeMatcherListBoxItem item = new RoomTypeMatcherListBoxItem();
                item.Initialize(mChar, types);
                ListBoxX.Items.Add(item);
            });
        }

        private void InitializeItems(List<ELiteRoomTypeMatchItem> matches, List<ELiteRoomTypeItem> types)
        {
            ListBoxX.Items.Clear();
            matches.ForEach(match =>
            {
                RoomTypeMatcherListBoxItem item = new RoomTypeMatcherListBoxItem();
                item.Initialize(match, types);
                //item.ComboBoxX.SelectionChanged += ComboBoxX_SelectionChanged;
                //item.TextBoxX.TextChanged += TextBoxX_TextChanged;
                ListBoxX.Items.Add(item);
            });
        }

        private void RadioButton_Avaliable_Checked(object sender, RoutedEventArgs e)
        {
            SQLiteTransaction tran = _Conn.BeginTransaction();
            InitializeItems(_Conn.GetMatchedRoomTypeMatchChar(), _Conn.GetAllRoomType());
            tran.Commit();
            b_update.IsEnabled = true;
            b_insert.IsEnabled = false;
        }

        private void RadioButton_Unavailable_Checked(object sender, RoutedEventArgs e)
        {
            SQLiteTransaction tran = _Conn.BeginTransaction();
            LoadItems(_Conn.GetUnmatchedTypeMatchChar(), _Conn.GetAllRoomType());
            tran.Commit();
            b_update.IsEnabled = false;
            b_insert.IsEnabled = true;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            SQLiteTransaction tran = _Conn.BeginTransaction();
            foreach(RoomTypeMatcherListBoxItem item in ListBoxX.Items)
            {
                if (!item.IsChanged) continue;
                item.MatchItem.UpdateIn(_Conn);
            }
            tran.Commit();
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            SQLiteTransaction tran = _Conn.BeginTransaction();
            foreach (RoomTypeMatcherListBoxItem item in ListBoxX.Items)
            {
                if (!item.IsChanged) continue;
                item.MatchItem.InsertTo(_Conn);
            }
            LoadItems(_Conn.GetUnmatchedTypeMatchChar(), _Conn.GetAllRoomType());
            tran.Commit();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class RoomTypeMatcher
    {
        public static void ShowRoomTypeMatcher(ELiteConnection conn)
        {
            if (conn == null) return;
            RoomTypeMatcherWindow window = new RoomTypeMatcherWindow();
            window.Initialize(conn);
            window.ShowDialog();
        }
    }
}
