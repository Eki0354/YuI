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
using System.Runtime.InteropServices;
using System.Text;
using Ran;
using FFElf;

namespace YuI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int nMaxCount);

        public static Ran.MainWindow WinRan = null;
        static List<Dictionary<string, string>> order_items = new List<Dictionary<string, string>>();
        static List<DataTable> order_rooms = new List<DataTable>();

        public MainWindow()
        {
            InitializeComponent();
            RegisterKevin();
            WinRan = new Ran.MainWindow();
            if (WinRan.ShowDialog() != true)
                Application.Current.Shutdown();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.IsDMRegistered)
            {
                switch (OutlookElf.Init())//初始化自动填装邮件内容的类
                {
                    case 0:
                        MessageBox.Show(
                            "请先运行火狐浏览器并打开Outlook邮箱页面(仅Mrs Panda账号有效)。");
                        Environment.Exit(0);
                        break;
                    case 1:
                        break;
                    case 2:
                        if (MessageBox.Show(
                            "绑定火狐窗口失败！\r\nOK-邮件功能不可用\r\nCancel-退出重试",
                            "提示板", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                            Environment.Exit(0);
                        else
                            menu_ThrowCoin.IsEnabled = false;
                        break;
                    case 3:
                        if (MessageBox.Show("YuI运行目录下不存在-DMImages-文件夹！\r\n" +
                            "OK-邮件功能不可用\r\nCancel-退出重试",
                            "提示板", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                            Environment.Exit(0);
                        else
                            menu_ThrowCoin.IsEnabled = false;
                        break;
                }
            }
            //_pageSale = new Page_Sale();
            //_pageRoom = new Page_Room();
            _pageRes = new Page_Reservation();
            if (MMC.LoggedInSID < 0) Application.Current.Shutdown();
            InitNotifyIcon();
            MainRibbon_SelectionChanged(null, null);
            this.StateChanged += MainWindow_StateChanged;
            this.Closed += RemoveNotifyIcon;
            //_TimerBackupDBCallback(null);
            _pageRes.MementoAPTX = this.MementoAPTX;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            //窗口最小化时隐藏任务栏图标
            if (WindowState == WindowState.Minimized) this.HideWindow(null, null);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MMC.Close();
        }

        private void MainRibbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mainFrame.Content = _pageRes;
            #region Tab
            /*if (MainRibbon.SelectedItem.GetType() != typeof(TabItem)) { return; }
            TabItem rt = MainRibbon.SelectedItem as TabItem;
            if (mainFrame.Content != null && _SelectedRibbonTab == rt) return;
            _SelectedRibbonTab = rt;
            switch (_SelectedRibbonTab.Header.ToString())
            {
                case "房态":
                    //mainFrame.Content = _pageRoom;
                    break;
                case "订单管理":
                    mainFrame.Content = _pageRes;
                    break;
                case "销售":
                    mainFrame.Content = _pageSale;
                    break;
            }*/
            #endregion
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) ||
                    e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) &&
                    e.KeyboardDevice.IsKeyDown(Key.F))
            {
                _pageRes.FindRes();
            }
            if (e.KeyboardDevice.IsKeyDown(Key.RightCtrl) &&
                e.KeyboardDevice.IsKeyDown(Key.D))
            {
                RibbonButtonShowResDetails_Click(null, null);
            }
            if ((e.KeyboardDevice.IsKeyDown(Key.RightCtrl) ||
                e.KeyboardDevice.IsKeyDown(Key.LeftCtrl)) &&
                (e.KeyboardDevice.IsKeyDown(Key.LeftShift) ||
                e.KeyboardDevice.IsKeyDown(Key.RightShift)) &&
                e.KeyboardDevice.IsKeyDown(Key.E))
            {
                _pageRes.EmailSendButton_Click(null, null);
            }
        }
        
        private void RibbonButtonShowResDetails_Click(object sender, RoutedEventArgs e)
        {
            _pageRes.lvRes.Visibility = Visibility.Visible;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(100);
            int times = 0;
            IntPtr hwnd = GetForegroundWindow();
            while (times < 3 && hwnd.ToInt32() == 0)
            {
                System.Threading.Thread.Sleep(500);
                hwnd = GetForegroundWindow();
                times++;
            }
            int length = GetWindowTextLength(hwnd);
            StringBuilder windowName = new StringBuilder(length + 1);
            GetWindowText(hwnd, windowName, windowName.Capacity);
            switch (windowName.ToString())
            {
                case string name when name.EndsWith("Excel"):
                    _pageRes.SetResDetailsCopy();
                    break;
                default:
                    break;
            }
        }

        private void Menu_Synchronize_Click(object sender, RoutedEventArgs e)
        {
            MMC.Backup(null);
        }
        
        #region RegisterKevin

        private static void RegisterKevin()
        {
            if (MMC.IsExisted("info_staff", "Nickname='Kevin'")) return;
            int maxSID = RegisterWindow.GetMaxSID() + 1;
            Dictionary<string, object> aptxDict = new Dictionary<string, object>()
            {
                {"sid", maxSID },
                {"Nickname","Kevin" },
                {"Sex",0 },
                {"Birth", new DateTime(1361, 02, 01) },
                {"Identity", 1 },
                {"Password", APTXItem.GetEncryptedPassword("loveandpeace" + "Memento") },
                {"Salt",  "Memento" }
            };
            MMC.Insert("info_staff", aptxDict);
        }

        #endregion

        private void ChangeHWPasswordMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var password = Microsoft.VisualBasic.Interaction.InputBox(
                "粘贴要变更为的密码字符（不能含有'='符号）", "合鸟");
            if (string.IsNullOrEmpty(password)) return;
            try
            {
                _pageRes._XmlReader.ChangeHWPasswordTo(password);
                Pop("HostelWorld登录密码已经被成功更改并保存！");
            }
            catch
            {
                Pop("变更密码失败！\r\n请给Eki发邮件或微信消息！");
            }
        }
    }
}
