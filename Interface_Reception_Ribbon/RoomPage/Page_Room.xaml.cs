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
using System.Data;
using ELite;


namespace Interface_Reception_Ribbon
{
    /// <summary>
    /// Page_Room.xaml 的交互逻辑
    /// </summary>
    public partial class Page_Room : Page
    {
        #region PROPERTY

        //private static ELiteConnection _Conn = MainWindow._Conn;
        DateTime _Date_Selected_Start;
        DateTime _Date_Selected_End;

        #endregion

        public Page_Room()
        {
            InitializeComponent();
        }

        #region DATAGRID

        DataTable _seDataTable = new DataTable();
        DataTable _monthDataTable = new DataTable();

        public void ChangeDisplayDate(DateTime date_start, DateTime date_end)
        {
            if (dg_room == null)
                return;
            if (_Date_Selected_Start == date_start && _Date_Selected_End == date_end)
                return;
            _Date_Selected_Start = date_start;
            _Date_Selected_End = date_end;
            UpdateSEDataTable(_Date_Selected_Start, _Date_Selected_End);
            dg_room.ItemsSource = _seDataTable.DefaultView;
        }

        private void UpdateSEDataTable(DateTime date_start, DateTime date_end)
        {

        }

        #endregion

        #region CONN

        


        #endregion
    }
}
