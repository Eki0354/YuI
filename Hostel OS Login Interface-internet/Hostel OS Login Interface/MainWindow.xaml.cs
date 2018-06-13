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

namespace Hostel_OS_Login_Interface
{
    public partial class MainWindow : Window
    {
        private string defaultText_Account = "Please input your account";
        private string defaultText_Password = "Please input your password";
        public MainWindow()
        {
            InitializeComponent();
            tbAccount.Text = defaultText_Account;
            tbPassword.Text = defaultText_Password;
        }

        private void MainGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SignupMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ForgotPasswordMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WechatButton_Click(object sender, RoutedEventArgs e)
        {
            wechatPopup.IsOpen = false;
            wechatPopup.IsOpen = true;
        }

        private void TextBoxAccount_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbAccount.Text == defaultText_Account) { tbAccount.Text = ""; }
            tbAccount.Foreground = new SolidColorBrush(Color.FromRgb(238, 241, 244));
        }

        private void TextBoxPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbPassword.Text == defaultText_Password) { tbPassword.Text = ""; }
            tbPassword.Foreground = new SolidColorBrush(Color.FromRgb(238, 241, 244));
        }

        private void TextBoxAccount_LostFocus(object sender, RoutedEventArgs e)
        {
            if(tbAccount.Text=="") { tbAccount.Text = defaultText_Account; }
            if (tbAccount.Text == defaultText_Account) { tbAccount.Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 180)); }
        }

        private void TextBoxPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbPassword.Text == "") { tbPassword.Text = defaultText_Password; }
            if (tbPassword.Text == defaultText_Password) { tbPassword.Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 180)); }
        }

        private string password
        {
            get;
            set;
        }

        private void TextBoxPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            password = tbPassword.Text;
            if (password != defaultText_Password) { tbPassword.Text = "".PadRight(password.Length, Convert.ToChar("*")); }
            tbPassword.Select(password.Length, 0);
        }

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
        
        private void RemPassCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if(remPassCheckBox.IsChecked==false ) { autoCheckBox.IsChecked = false; }
        }

        private void AutoCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if(autoCheckBox.IsChecked==true) { remPassCheckBox.IsChecked = true; }
        }
    }
    public class ImageButton : Button
    {
        private string m_imagepath;
        public string ImgPath
        {
            get { return m_imagepath; }
            set { m_imagepath = value; }
        }

    }
}
