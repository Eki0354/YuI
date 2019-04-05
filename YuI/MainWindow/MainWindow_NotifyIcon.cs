using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace YuI
{
    public partial class MainWindow
    {
        #region 通知栏

        NotifyIcon NotifyIcon;
        bool IsShining = false;


        private void InitNotifyIcon()
        {
            this.NotifyIcon = new NotifyIcon
            {
                BalloonTipText = "咕", //设置程序启动时显示的文本
                Text = "Y u I",//最小化到托盘时，鼠标移动到图标上时显示的文本
                Icon = new System.Drawing.Icon(System.Windows.Application.GetResourceStream(
                    new Uri("pack://application:,,,/Resources/Images/yui.ico")).Stream),//程序图标
                Visible = true
            };

            //右键菜单--打开菜单项
            MenuItem menuOpen = new MenuItem("显示主界面");
            menuOpen.Click += ShowWindow;
            //右键菜单--退出菜单项
            MenuItem menuExit = new MenuItem("退出");
            menuExit.Click += CloseWindow;
            menuExit.Click += RemoveNotifyIcon;
            //关联托盘控件
            MenuItem[] childen = new MenuItem[] { menuOpen, menuExit };
            NotifyIcon.ContextMenu = new ContextMenu(childen);

            NotifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
            this.NotifyIcon.ShowBalloonTip(1000);
        }

        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            /*
             * 这一段代码需要解释一下:
             * 窗口正常时双击图标执行这段代码是这样一个过程：
             * this.Show()-->WindowState由Normail变为Minimized-->Window_StateChanged事件执行(this.Hide())-->WindowState由Minimized变为Normal-->窗口隐藏
             * 窗口隐藏时双击图标执行这段代码是这样一个过程：
             * this.Show()-->WindowState由Normail变为Minimized-->WindowState由Minimized变为Normal-->窗口显示
             */
            this.Show();
            this.WindowState = WindowState.Minimized;
            this.WindowState = WindowState.Normal;
        }

        private void ShowWindow(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
            this.ShowInTaskbar = true;
            this.Activate();
        }

        private void HideWindow(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Visibility = Visibility.Hidden;
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void RemoveNotifyIcon(object sender, EventArgs e)
        {
            this.NotifyIcon.Visible = false;
        }

        #endregion
    }
}
