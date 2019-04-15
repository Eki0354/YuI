using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.OleDb;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Collections.Generic;
using MMC = MementoConnection.MMConnection;
using MementoConnection;
using System.Data;
using System.Windows.Interop;

namespace Ran
{
    public partial class MainWindow : Window
    {
        public List<APTXItem> APTXItems
        {
            get { return (List<APTXItem>)GetValue(APTXItemsProperty); }
            set { SetValue(APTXItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for APTXItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty APTXItemsProperty =
            DependencyProperty.Register("APTXItems", typeof(List<APTXItem>), typeof(MainWindow), new PropertyMetadata(null));
        
        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionEventHandler);
            SettingWindow.LoadSettings();
            InitializeComponent();
        }
        
        public static void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                string logDirectory = Environment.CurrentDirectory + @"\Logs";
                if (!Directory.Exists(logDirectory)) Directory.CreateDirectory(logDirectory);
                string logPath = logDirectory +
                    string.Format(@"\log{0}.txt", DateTime.Now.Ticks.ToString());
                Console.WriteLine(logPath);
                using (FileStream fs = new FileStream(logPath, FileMode.OpenOrCreate))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write(e.ExceptionObject.ToString());
                    sw.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Owner is null) Application.Current.Shutdown();
            InitAPTX();
        }

        private void InitAPTX()
        {
            List<APTXItem> items = new List<APTXItem>();
            foreach (DataRow row in MMC.GetStaffList.Rows)
            {
                items.Add(APTXItem.FromDataRow(row));
            }
            APTXItems = items.GetDisruptedAPTX();
            Dictionary<string, string> passwords = SavedPasswordElf.ReadPasswords();
            items.ForEach(item =>
            {
                if (passwords.ContainsKey(item.Nickname))
                    item.SavedPassword = passwords[item.Nickname];
            });
        }

        private void MainGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        #region  加密、解密数据库
        private void EncryptDataBase()
        {
            FileStream fs = new FileStream(Environment.CurrentDirectory + @"\accounts.db", FileMode.Open);
            byte[] dbbytes = new byte[fs.Length];
            fs.Read(dbbytes, 0, (int)fs.Length);
            for (int i = 1; i < 16; i += 2)
            {
                if(dbbytes[i]==255)
                {
                    dbbytes[i] = 0;
                }
                else
                {
                    dbbytes[i]++;
                }
            }
            fs.Seek(0, SeekOrigin.Begin);
            fs.Write(dbbytes, 0, (int)fs.Length);
            fs.Flush();
            fs.Close();
        }

        private void UnEncryptDataBase()
        {
            FileStream fs = new FileStream(Environment.CurrentDirectory + @"\accounts.db", FileMode.Open);
            byte[] dbbytes = new byte[fs.Length];
            fs.Read(dbbytes, 0, (int)fs.Length);
            if (dbbytes[1] != 1)
            {
                for (int i = 1; i < 16; i += 2)
                {
                    if (dbbytes[i] == 0)
                    {
                        dbbytes[i] = 255;
                    }
                    else
                    {
                        dbbytes[i]--;
                    }
                }
                fs.Seek(0, SeekOrigin.Begin);
                fs.Write(dbbytes, 0, (int)fs.Length);
            }
            fs.Flush();
            fs.Close();
        }

        #endregion

        //登录操作，包括动画显示、账号验证、身份判断和启动相应系统
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if(tbPassword.Password is string password && cbAccount.SelectedItem is APTXItem aptx &&
                aptx.EqualsPassword(password))
            {
                if (cbSavePassword.IsChecked == true)
                    SavedPasswordElf.SavePassword(aptx.Nickname, password);
                MMC.LogIn(aptx.SID);
                this.Close();
            }
            else
            {
                ppLogin.IsOpen = false;
                ppLogin.IsOpen = true;
            }
        }

        private void StartSystem(string account, int level, string number)
        {

        }
        
        //菜单项事件
        private void menu_signup_Click(object sender, RoutedEventArgs e)
        {
            configPopup.IsOpen = false;
            RegisterWindow.Summon();
            InitAPTX();
        }
        
        private void menu_forgotpassword_Click(object sender, RoutedEventArgs e)
        {
            configPopup.IsOpen = false;
            fpPopup.IsOpen = false;
            fpPopup.IsOpen = true;
        }

        private void menu_options_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow sw = new SettingWindow();
            sw.ShowDialog();
        }

        private void menu_about_Click(object sender, RoutedEventArgs e)
        {
            configPopup.IsOpen = false;
            //aboutPopup.IsOpen = false;
            //aboutPopup.IsOpen = true;
        }

        private void WechatButton_Click(object sender, RoutedEventArgs e)
        {
            wechatPopup.IsOpen = false;
            wechatPopup.IsOpen = true;
        }

        //面板控制按钮事件
        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            configPopup.IsOpen = false;
            configPopup.IsOpen = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        
        //单选框事件
        private void RemPassCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if(cbSavePassword.IsChecked==false ) { cbAutoLogin.IsChecked = false; }
        }

        private void AutoCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if(cbAutoLogin.IsChecked==true) { cbSavePassword.IsChecked = true; }
        }

        private void cbAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox cb)
            {
                APTXItem aptx = cb.SelectedItem as APTXItem;
                tbPassword.Password = aptx.SavedPassword;
            }
        }
    }
    public class ImageButton : Button
    {
        public string ImgPath { get; set; }
    }

    public class BGChangeButton : Button
    {
        public string NormalBG { get; set; }
        public string MouseOverBG { get; set; }
        public string PressedBG { get; set; }
        public string DisabledBG { get; set; }
    }
}
