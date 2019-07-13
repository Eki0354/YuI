using System;
using System.Diagnostics;
using System.Windows;
using FFElf;
using System.Threading;
using System.Threading.Tasks;

namespace YuI
{
    public partial class MainWindow
    {
        private void menu_update_Click(object sender, RoutedEventArgs e)
        {
            string currentPath = Environment.CurrentDirectory;
            Process process = new Process();
            process.StartInfo.FileName = currentPath + "\\Update.exe";
            process.StartInfo.WorkingDirectory = currentPath;
            process.Start();
        }

        private void MenuSwitchStaff_Click(object sender, RoutedEventArgs e)
        {
            (new Ran.MainWindow()).ShowDialog();
            _pageRes.tb_aptx.Text = MementoAPTX.Nickname;
        }

        private void menu_options_Click(object sender, RoutedEventArgs e)
        {
            new tmpApp.Form1().Show();
        }

        private void Menu_RegisterStaff_Click(object sender, RoutedEventArgs e)
        {
            if (MementoAPTX.Identity < 1)
                MessageBox.Show("当前登录账号权限不足！\r\n" +
                    "请切换至具有-店长-及以上权限的账号进行此操作。");
            else
                (new RegisterWindow()).ShowDialog();
        }

        private void Menu_ThrowCoin_Click(object sender, RoutedEventArgs e)
        {
            if (_pageRes.IsCERes)
            {
                var email = _pageRes.GetEmail(false);
                if (email is null)
                {
                    MessageBox.Show("自动生成邮件内容出错！");
                }
                else
                {
                    OutlookElf.SetConfirmEmail(email);
                }
            }
            else
            {
                MessageBox.Show("此订单无需确认或邮箱地址为空！");
            }
        }

        private void Menu_OpenSendWindow_Click(object sender, RoutedEventArgs e)
        {
            EmailWindow.Summon(_pageRes.GetEmail(true));
        }

        private void Menu_ChangeOutlookPassword_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_ChangeSignature_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_HowToFindSignature_Click(object sender, RoutedEventArgs e)
        {

        }
        
        private void Menu_NormalBody_Click(object sender, RoutedEventArgs e)
        {
            var email = _pageRes.GetEmail(false);
            if (email is null)
                MessageBox.Show("此订单无法生成确认邮件正文");
            else
                Clipboard.SetText(email.Body);
        }
    }
}
