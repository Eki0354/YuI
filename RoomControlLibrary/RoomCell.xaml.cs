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

namespace RoomCell
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class RoomCell : UserControl
    {
        public static DependencyProperty IsBookedProperty;
        public static DependencyProperty IsEnableProperty;
        public static DependencyProperty ExchangeNumber;
        public static DependencyProperty OrderState;
        public RoomCell()
        {
            InitializeComponent();
            DisplayMode = DisplayModes.Grid;
        }
        public void RoomCell_SourceUpdated()
        {

        }
    }
    
}
