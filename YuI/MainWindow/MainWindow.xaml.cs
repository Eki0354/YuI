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
        public static Ran.MainWindow WinRan = null;

        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Ran.MainWindow.UnhandledExceptionEventHandler);
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _pageSale = new Page_Sale();
            _pageRoom = new Page_Room();
            _pageRes = new Page_Reservation();
            SummonRan();
            if (MMC.LoggedInSID < 0) Application.Current.Shutdown();
            InitNotifyIcon();
            MainRibbon_SelectionChanged(null, null);
            InitializeControls();
            Ledros.WPFClipboard.InitClipboardWatcher(this);
            this.StateChanged += MainWindow_StateChanged;
            this.Closed += RemoveNotifyIcon;
        }
        
        private void SummonRan()
        {
            WinRan = new Ran.MainWindow();
            WinRan.Owner = this;
            WinRan.ShowDialog();
            _pageRes.MementoAPTX = this.MementoAPTX;
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
                        _pageRes.EmailAddressCopyButton_Click(null, null);
                        break;
                    case Key.F2:
                        _pageRes.EmailThemeCopyButton_Click(null, null);
                        break;
                    case Key.F3:
                        _pageRes.EmailBodyCopyButton_Click(null, null);
                        break;
                }
                if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) ||
                    e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) &&
                    e.KeyboardDevice.IsKeyDown(Key.F))
                {
                    _pageRes.FindRes();
                }
                if(e.KeyboardDevice.IsKeyDown(Key.RightCtrl) && 
                    e.KeyboardDevice.IsKeyDown(Key.D))
                {
                    RibbonButtonShowResDetails_Click(null, null);
                }
                if((e.KeyboardDevice.IsKeyDown(Key.RightCtrl) || 
                    e.KeyboardDevice.IsKeyDown(Key.LeftCtrl)) &&
                    (e.KeyboardDevice.IsKeyDown(Key.LeftShift) ||
                    e.KeyboardDevice.IsKeyDown(Key.RightShift))&&
                    e.KeyboardDevice.IsKeyDown(Key.S))
                {
                    _pageRes.EmailSendButton_Click(null, null);
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

        private void MenuSwitchStaff_Click(object sender, RoutedEventArgs e)
        {
            SummonRan();
        }

        private void RibbonButtonShowResDetails_Click(object sender, RoutedEventArgs e)
        {
            _pageRes.lvRes.Visibility = Visibility.Visible;
        }
    }
}
