using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ELite;
using ELite.Reservation;
using Microsoft.VisualBasic;

namespace EControlsLibrary
{
    /// <summary>
    /// ResListBox.xaml 的交互逻辑
    /// </summary>
    public partial class ResListView : UserControl
    {
        ELiteConnection _Conn;

        public ELiteListBoxResItem SelectedResItem
        {
            get
            {
                ListBoxResBalloon balloon = lb_res.SelectedItem as ListBoxResBalloon;
                if (balloon == null) return null;
                return balloon.ResItem;
            }
        }

        #region Initialize

        public ResListView()
        {
            InitializeComponent();
        }

        public void InitializeFrom(ELiteConnection conn, SelectionChangedEventHandler handler = null)
        {
            if (conn == null) return;
            _Conn = conn;
            InitializeListBox();
            AddSelectionChangedEventHandler(handler);
        }

        public void InitializeListBox()
        {
            if (_Conn == null) return;
            _Conn.UnCheckedListBoxResItems().ForEach(item =>
            lb_res.Items.Add(new ListBoxResBalloon(item, pp_menu)));
            if (lb_res.Items.Count > 0)
            {
                object obj = lb_res.Items[lb_res.Items.Count - 1];
                lb_res.ScrollIntoView(obj);
            }
        }

        public void AddSelectionChangedEventHandler(SelectionChangedEventHandler handler)
        {
            if (handler == null) return;
            lb_res.SelectionChanged += handler;
        }

        public void DeleteSelectionChangedEventHandler(SelectionChangedEventHandler handler)
        {
            if (handler == null) return;
            lb_res.SelectionChanged -= handler;
        }

        #endregion

        #region Items

        public void Add(ELiteListBoxResItem res)
        {
            if (res == null) return;
            Dispatcher.Invoke(new Action(() =>
            {
                ListBoxResBalloon balloon = new ListBoxResBalloon(res, pp_menu);
                lb_res.Items.Insert(0, balloon);
                lb_res.SelectedItem = balloon;
                lb_res.ScrollIntoView(balloon);
            }));
        }

        public void Add(ListBoxResBalloon balloon)
        {
            if (balloon == null) return;
            Dispatcher.Invoke(new Action(() =>
            {
                lb_res.Items.Insert(0, balloon);
                lb_res.SelectedItem = balloon;
                lb_res.ScrollIntoView(balloon);
            }));
        }
        
        /// <summary> 此方法为移除ResListBoxItem项的根方法，其他参数的Remove方法实现过程都将调用此方法。 </summary>
        public void Remove(int index)
        {
            if (index < 0 || lb_res.Items.Count <= index) return;
            lb_res.Items.RemoveAt(index);
            if (index > 0) index--;
            if (lb_res.Items.Count > 0) lb_res.SelectedIndex = index;
        }
        
        public void Remove(ListBoxResBalloon balloon)
        {
            if (balloon == null || !lb_res.Items.Contains(balloon)) return;
            Remove(lb_res.Items.IndexOf(balloon));
        }

        public void Remove(IList IL)
        {
            if (IL.Count < 1) return;
            int index = lb_res.Items.IndexOf(IL[0]);
            List<ListBoxResBalloon> balloons = new List<ListBoxResBalloon>();
            foreach(ListBoxResBalloon balloon in IL)
            {
                balloons.Add(balloon);
            }
            balloons.ForEach(balloon => lb_res.Items.Remove(balloon));
            if (index > 0) index--;
            if (lb_res.Items.Count > 0) lb_res.SelectedIndex = index;
        }

        public void RemoveSelection()
        {
            Remove(lb_res.SelectedIndex);
        }

        public void Clear()
        {
            lb_res.Items.Clear();
        }

        public bool Find(string keyword = "")
        {
            if (string.IsNullOrEmpty(keyword))
                keyword = Interaction.InputBox("请输入要查找的订单号或客人姓名：", "提示", "");
            if (string.IsNullOrEmpty(keyword)) return false;
            foreach (ListBoxResBalloon balloon in lb_res.Items)
            {
                if (balloon == null || (!balloon.EqualsToResNumber(keyword) &&
                    balloon.ResItem.FullName.ToLower().IndexOf(keyword.ToLower()) < 0)) continue;
                balloon.ResItem.IsSearchResult = true;
                Dispatcher.Invoke(new Action(() =>
                {
                    lb_res.SelectedItem = balloon;
                }));
                return true;
            }
            List<ELiteListBoxResItem> resList = _Conn.FindResByResNumberOrFullName(keyword);
            if (resList.Count < 1) return false;
            resList.ForEach(item =>
            {
                item.IsSearchResult = true;
                Add(item);
            });
            Dispatcher.Invoke(new Action(() =>
            {
                lb_res.SelectedItem = resList.Last();
            }));
            return true;
        }

        #endregion

        #region ControlsEvents

        private void ClearButton_Click(object sender, EventArgs e)
        {
            Clear();
            this._Conn.UpdateResAllChecked();
        }

        private void MarkButton_Click(object sender, RoutedEventArgs e)
        {
            IList IL = lb_res.SelectedItems;
            foreach(ListBoxResBalloon balloon in IL)
            {
                _Conn.UpdateResChecked(balloon.ResItem.ResNumber);
            }
            Remove(IL);
        }

        private void InvalidButton_Click(object sender, RoutedEventArgs e)
        {
            IList IL = lb_res.SelectedItems;
            foreach (ListBoxResBalloon balloon in IL)
            {
                _Conn.InvalidResByNumber(balloon.ResItem.ResNumber);
            }
            Remove(IL);
        }

        private void InvalidAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (ListBoxResBalloon balloon in lb_res.Items)
            {
                _Conn.InvalidResByNumber(balloon.ResItem.ResNumber);
            }
            Clear();
        }

        private void ResListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pp_menu.IsOpen = false;
            IList iL = lb_res.SelectedItems;
            switch (iL.Count)
            {
                case 0:

                    break;
                case 1:
                    object obj = lb_res.SelectedItem;
                    if (obj == null) return;
                    lb_res.ScrollIntoView(obj);
                    break;
                default:
                    
                    break;
            }
        }

        #endregion

    }
}
