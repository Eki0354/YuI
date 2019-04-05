using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using System.Windows.Controls.Ribbon;
using MementoConnection;
using MMC = MementoConnection.MMConnection;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading.Tasks;

namespace YuI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        RibbonTab _SelectedRibbonTab = null;
        static List<Dictionary<string, string>> order_items = new List<Dictionary<string, string>>();
        static List<DataTable> order_rooms = new List<DataTable>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var w = new Ran.MainWindow();
            w.Owner = this;
            w.ShowDialog();
            if (MMC.LoggedInSID < 0) Application.Current.Shutdown();
            //if (Process.GetProcessesByName("Ran").Length < 1) Environment.Exit(0);
            _pageSale = new Page_Sale();
            _pageRoom = new Page_Room();
            _pageRes = new Page_Reservation();
            InitNotifyIcon();
            MainRibbon_SelectionChanged(null, null);
            InitializeControls();
            this.StateChanged += MainWindow_StateChanged;
            this.Closed += RemoveNotifyIcon;
        }
        
        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            //窗口最小化时隐藏任务栏图标
            if (WindowState == WindowState.Minimized) this.Hide();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MMC.Close();
        }

        private void InitializeControls()
        {
            InitializeControls_Room();
            InitializeControls_ResGroup();
        }

        private void MainRibbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainRibbon.SelectedItem.GetType() != typeof(RibbonTab)) { return; }
            RibbonTab rt = MainRibbon.SelectedItem as RibbonTab;
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

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
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
            string currentPath = Environment.CurrentDirectory;
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = currentPath + "\\Update.exe";
            process.StartInfo.WorkingDirectory = currentPath;
            process.Start();
        }

    }
}
