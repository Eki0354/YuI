using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomCell
{
    //房间单元格显示风格，依赖属性，用于不同视图
    public enum DisplayModes { Array, Grid }
    public partial class RoomCell : UserControl
    {
        public DisplayModes DisplayMode
        {
            get { return (DisplayModes)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }
        
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(DisplayModes), typeof(RoomCell), new PropertyMetadata(0));


    }
}
