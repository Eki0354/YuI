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
using System.Data.SQLite;
using System.Windows.Controls.Ribbon;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using ELite;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using ELite.Reservation;
using EControlsLibrary;
using Microsoft.Win32;
using System.IO;

namespace Interface_Reception_Ribbon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private static ELiteConnection _Conn;
        public static ELiteConnection Conn => _Conn;
        public static string AppDataPath =>
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HROS";
        public string DataBasePath => AppDataPath + "\\database";
        public string DataBaseFullPath => DataBasePath + "\\mp.db";
        string _OneDrivePath;
        bool _IsNeedUpdate = false;
        RibbonTab _SelectedRibbonTab = null;
        //ResAssist _ResAssist;
        //ResGetter _ResGetter;

        static List<Dictionary<string, string>> order_items = new List<Dictionary<string, string>>();
        static List<DataTable> order_rooms = new List<DataTable>();
        public MainWindow()
        {
            menu_update_Click(null, null);
            InitializeComponent();
            _Conn = new ELiteConnection(DataBaseFullPath, "");
            _Conn.Open();
            _pageSale = new Page_Sale();
            _pageRoom = new Page_Room();
            _pageRes = new Page_Reservation();

        }

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainRibbon_SelectionChanged(null, null);
            //_ResAssist = new ResAssist(_Conn);
            //_ResGetter = new ResGetter(_Conn);
            InitializeControls();
        }

        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_Conn != null)
                _Conn.Close();
            _Conn = null;
        }

        private void InitializeControls()
        {
            InitializeControls_Room();
            InitializeControls_ResGroup();
        }

        private void MainRibbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainRibbon.SelectedItem.GetType() != typeof(RibbonTab)) { return; }
            RibbonTab rt = mainRibbon.SelectedItem as RibbonTab;
            if (mainFrame.Content != null && _SelectedRibbonTab == rt) return;
            _SelectedRibbonTab = rt;
            switch (_SelectedRibbonTab.Header.ToString())
            {
                case "房态":
                    mainFrame.Content = _pageRoom;
                    break;
                case "订单管理":
                    mainFrame.Content = _pageRes;
                    break;
                case "销售":
                    mainFrame.Content = _pageSale;
                    break;
            }
        }

        private void RibbonWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (_SelectedRibbonTab == null) return;
            if (_SelectedRibbonTab.Header.ToString() == "订单管理")
            {
                switch (e.Key)
                {
                    case Key.F1:
                        CopyEmailReplies("0");
                        break;
                    case Key.F2:
                        CopyEmailReplies("1");
                        break;
                    case Key.F3:
                        CopyEmailReplies("2");
                        break;
                }
                if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) ||
                    e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) &&
                    e.KeyboardDevice.IsKeyDown(Key.F))
                {
                    _pageRes.FindRes();
                }
            }
        }

        private void menu_update_Click(object sender, RoutedEventArgs e)
        {
            string currentPath = "C:\\Program Files (x86)\\E.A\\Hostel Reception Operation System";// Environment.CurrentDirectory;
            if (currentPath.Contains("OneDrive"))
            {
                EMsgBox.ShowMessage("当前在编译环境运行，不需要更新！");
                return;
            }
            RegistryKey rKey = Registry.CurrentUser.OpenSubKey("Environment");
            _OneDrivePath = rKey.GetValue("OneDrive") as string;
            string usPath = _OneDrivePath + "\\C#\\Hostel Reception Operation System\\update.ini";
            if (!File.Exists(usPath))
            {
                EMsgBox.ShowMessage("更新配置文件不存在！");
                return;
            }
            FileStream fs = new FileStream(usPath, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string settingString = sr.ReadLine();
            sr.Close();
            fs.Close();
            _IsNeedUpdate = sender != null ||
                settingString.Substring(settingString.IndexOf("=") + 1) != "0";
            if (!_IsNeedUpdate) return;
            this.Close();
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = currentPath + "\\Update.exe";
            process.StartInfo.WorkingDirectory = currentPath;
            process.Start();
        }

    }
}
