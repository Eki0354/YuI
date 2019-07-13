using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using MMC = MementoConnection.MMConnection;

namespace YuI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool IsDMRegistered { get; private set; } = false;
        public static readonly int WM_UNIQUE = 0x0354;
        [DllImport("user32.dll")]
        static extern IntPtr PostMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);

        static System.Threading.Timer TimerBackupDB = new System.Threading.Timer(
            _CheckOneDriveCallback, null, 0, 600000);

        public static bool IsSuicidable() =>
            DateTime.Now.TimeOfDay >= new TimeSpan(23, 50, 00);

        protected override void OnStartup(StartupEventArgs e)
        {
            if (IsSuicidable()) Environment.Exit(0);
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(
                    Ran.MainWindow.UnhandledExceptionEventHandler);
            Process cp = Process.GetCurrentProcess();
            if(Process.GetProcessesByName(cp.ProcessName) is Process[] pros && pros.Length > 1)
            {
                PostMessage((IntPtr)FindWindow(null, "YuI"), 
                    WM_UNIQUE, IntPtr.Zero, IntPtr.Zero);
                Environment.Exit(0);
            }
            else
            {
                IsDMRegistered = RegisterDM();
                base.OnStartup(e);
            }
        }

        private static void _CheckOneDriveCallback(object state)
        {
            Process[] odPros = Process.GetProcessesByName("OneDrive");
            if (odPros == null || odPros.Length < 1)
            {
                MessageBox.Show("检测到OneDrive未运行！\r\n请在开始菜单搜索\"OneDrive\"并启动后再重新打开YuI。");
                Environment.Exit(0);
            }
            MMC.Backup(null);
        }
        
        [DllImport("dm.dll")]
        public static extern int DllRegisterServer();
        [DllImport("dm.dll")]
        public static extern int DllUnregisterServer();

        static bool RegisterDM()
        {
            try
            {
                string sourcePath = Environment.CurrentDirectory + @"\dm.dll";
                string destPath = Environment.GetFolderPath(
                    Environment.SpecialFolder.System) + @"\dm.dll";
                if (!File.Exists(destPath))
                    File.Copy(sourcePath, destPath, false);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show("首次注册操作火狐浏览器的组件需要以管理员权限运行！\r\n" +
                    "本次邮件功能不可用。");
                return false;
            }
            return (Registry.ClassesRoot.OpenSubKey(
                @"\WOW6432Node\CLSID\{26037A0E-7CBD-4FFF-9C63-56F2D0770214}") is null) ?
                (DllRegisterServer() > -1) : true;
        }

    }
}
