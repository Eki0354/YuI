using System.Diagnostics;
using System.Windows;

namespace YuI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //当前运行WPF程序的进程实例
            Process process = Process.GetCurrentProcess();
            if (Process.GetProcessesByName(process.ProcessName).Length > 1)
            {
                process.Kill();
                return;
            }  
            base.OnStartup(e);
        }
    }
}
