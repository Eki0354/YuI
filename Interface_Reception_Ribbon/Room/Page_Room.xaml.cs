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
using System.Data.OleDb;
using Interface_Reception_Ribbon.Room;


namespace Interface_Reception_Ribbon
{
    /// <summary>
    /// Page_Room.xaml 的交互逻辑
    /// </summary>
    public partial class Page_Room : Page
    {
        #region PROPERTY

        public static OleDbConnection _RoomConn;
        DateTime _date_selected_start;
        DateTime _date_selected_end;

        #endregion

        public Page_Room()
        {
            InitializeComponent();
            InitializeConn();
            _RoomConn.Open();
        }

        private void Page_Room_Closing(object sender, EventArgs e)
        {
            if (_RoomConn == null)
                return;
            _RoomConn.Close();
        }

        #region DATAGRID

        DataTable _seDataTable = new DataTable();
        DataTable _monthDataTable = new DataTable();

        public void ChangeDisplayDate(DateTime date_start, DateTime date_end)
        {
            if (dg_room == null)
                return;
            if (_date_selected_start == date_start && _date_selected_end == date_end)
                return;
            _date_selected_start = date_start;
            _date_selected_end = date_end;
            UpdateSEDataTable(_date_selected_start, _date_selected_end);
            dg_room.ItemsSource = _seDataTable.DefaultView;
        }

        private void UpdateSEDataTable(DateTime date_start, DateTime date_end)
        {

        }

        #endregion

        #region CONN

        private void InitializeConn()
        {
            string path = Environment.CurrentDirectory + "\\database\\rs.mdb";
            _RoomConn = new OleDbConnection();
            _RoomConn.ConnectionString = "Provider=Microsoft.Jet.OleDb.4.0;Data Source=" + path + 
                ";Jet Oledb:DataBase Password=Eki20150613";
        }


        #endregion
    }
}
