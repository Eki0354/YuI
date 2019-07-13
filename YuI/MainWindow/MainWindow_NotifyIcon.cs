using IOExtension;
using System;
using System.Collections.Generic;
using System.IO;
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

        static NotifyIcon NotifyIcon;
        bool IsShining = false;
        
        private void InitNotifyIcon()
        {
            NotifyIcon = new NotifyIcon
            {
                BalloonTipText = ReadUpdateTips(), //设置程序启动时显示的文本
                Text = "Y u I",//最小化到托盘时，鼠标移动到图标上时显示的文本
                Icon = new System.Drawing.Icon(System.Windows.Application.GetResourceStream(
                    new Uri("pack://application:,,,/Resources/Images/yui.ico")).Stream),//程序图标
                Visible = true
            };

            //右键菜单--打开菜单项
            MenuItem menuOpen = new MenuItem("签订契约！")
            {
                Enabled = false
            };
            menuOpen.Click += ShowWindow;
            //右键菜单--退出菜单项
            MenuItem menuExit = new MenuItem("退出");
            menuExit.Click += CloseWindow;
            menuExit.Click += RemoveNotifyIcon;
            //关联托盘控件
            MenuItem[] childen = new MenuItem[] { menuOpen, GetEmailTemplateMenuItem(), menuExit };
            NotifyIcon.ContextMenu = new ContextMenu(childen);

            NotifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
            NotifyIcon.ShowBalloonTip(120000);
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
            NotifyIcon.ContextMenu.MenuItems[0].Enabled =
                !NotifyIcon.ContextMenu.MenuItems[0].Enabled;
            this.Show();
            this.WindowState = WindowState.Minimized;
            this.WindowState = WindowState.Normal;
        }

        private void ShowWindow(object sender, EventArgs e)
        {
            NotifyIcon.ContextMenu.MenuItems[0].Enabled = false;
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
            this.Focus();
        }

        private void HideWindow(object sender, EventArgs e)
        {
            NotifyIcon.ContextMenu.MenuItems[0].Enabled = true;
            this.Hide();
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void RemoveNotifyIcon(object sender, EventArgs e)
        {
            NotifyIcon.Visible = false;
        }

        #endregion

        #region EmailTemplate

        private static MenuItem GetEmailTemplateMenuItem()
        {
            MenuItem rmi = new MenuItem
            {
                Text = "复制邮件模板"
            };
            ReadEmailTempletList().ForEach(et =>
            {
                MenuItem mi = new MenuItem
                {
                    Text = et
                };
                mi.Click += SetEmailTempletText;
                rmi.MenuItems.Add(mi);
            });
            return rmi;
        }

        public static List<string> ReadEmailTempletList()
        {
            string path = MementoPath.EmailTemplatesDirectory;
            if (!Directory.Exists(path)) return null;
            List<string> list = new List<string>();
            int startIndex = 0;
            int endIndex = 0;
            foreach (string filePath in Directory.GetFiles(path))
            {
                startIndex = filePath.LastIndexOf("\\") + 1;
                endIndex = filePath.LastIndexOf(".");
                list.Add(filePath.Substring(startIndex, endIndex - startIndex));
            }
            return list;
        }

        public static void SetEmailTempletText(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            System.Windows.Clipboard.SetText(ReadEmailTempletText(mi.Text));
            Pop("成功！", "已复制邮件模板：" + mi.Text);
        }

        public static string ReadEmailTempletText(string name)
        {
            string path = string.Format(@"{0}\{1}.txt",
                MementoPath.EmailTemplatesDirectory, name);
            string text;
            FileStream fs = new FileStream(path, FileMode.Open);
            text = new StreamReader(fs).ReadToEnd();
            fs.Close();
            return text;
        }

        #endregion
        
        public static void Pop(string msg)
        {
            NotifyIcon.BalloonTipTitle = "提示";
            NotifyIcon.BalloonTipText = msg;
            NotifyIcon.ShowBalloonTip(2000);
        }

        public static void Pop(string title, string msg)
        {
            NotifyIcon.BalloonTipTitle = title;
            NotifyIcon.BalloonTipText = msg;
            NotifyIcon.ShowBalloonTip(2000);
        }

        public string ReadUpdateTips() =>
            _pageRes._XmlReader.ReadValue("Main/UpdateTips").
            Replace("\\r", "\r").Replace("\\n", "\n");
    }
}
