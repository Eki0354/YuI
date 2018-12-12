using ELite.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// 用于在ListBox控件中显示简略订单列表的控件。
    /// </summary>
    public partial class ListBoxResBalloon : UserControl
    {
        public ELiteListBoxResItem ResItem { get; }
        Popup _MenuPopup { get; set; }
        
        public ListBoxResBalloon(ELiteListBoxResItem resItem, Popup markPopup)
        {
            InitializeComponent();
            ResItem = resItem ?? throw new Exception("初始化订单项失败：ListBoxResItem不能为空！");
            _MenuPopup = markPopup ?? throw new Exception("初始化订单项失败：MarkPopup不能为空！");
            tb_channel.Text = ResItem.Channel + (!ResItem.IsValid ? " 无效" : "") + (ResItem.IsSearchResult ? " 查" : "");
            tb_name.Text = ResItem.FullName;
        }

        public bool EqualsToResNumber(string resNumber)
        {
            return ResItem.ResNumber == resNumber;
        }

        public bool EqualsToResItem(ELiteListBoxResItem res)
        {
            return ResItem == res;
        }

        public bool EqualsToBalloon(ListBoxResBalloon balloon)
        {
            return ResItem == balloon.ResItem;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_MenuPopup == null) return;
            _MenuPopup.PlacementTarget = this;
            _MenuPopup.IsOpen = true;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_MenuPopup == null || _MenuPopup.IsMouseOver) return;
            _MenuPopup.PlacementTarget = null;
            _MenuPopup.IsOpen = false;
        }

        private void UserControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_MenuPopup == null || _MenuPopup.IsMouseOver) return;
            _MenuPopup.IsOpen = true;
        }
    }
}
