using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace YuI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static readonly int WM_UNIQUE = 0x0354;
        [DllImport("user32.dll")]
        static extern IntPtr PostMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);

        protected override void OnStartup(StartupEventArgs e)
        {
            Process cp = Process.GetCurrentProcess();
            if(Process.GetProcessesByName(cp.ProcessName) is Process[] pros && pros.Length > 1)
            {
                PostMessage((IntPtr)FindWindow(null, "YuI"), 
                    WM_UNIQUE, IntPtr.Zero, IntPtr.Zero);
                Environment.Exit(0);
            }
            else
            {
                base.OnStartup(e);
            }
        }
    }
}
