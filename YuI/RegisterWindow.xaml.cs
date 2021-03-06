﻿using Ran;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;
using MMC = MementoConnection.MMConnection;

namespace YuI
{
    /// <summary>
    /// RegisterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        public static int GetMaxSID()
        {
            if( MMC.GetMaxSIDStaff() is DataRow row)
                return APTXItem.FromDataRow(row).SID;
            else
                return 0;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            bool isOkToRegister = false;
            string nickname = tbNickname.Text;
            if (string.IsNullOrEmpty(nickname))
            {
                MessageBox.Show("昵称不能为空！");
                return;
            }
            string pw1 = pb1.Password;
            string pw2 = pb2.Password;
            if (string.IsNullOrEmpty(pw1) || string.IsNullOrEmpty(pw2))
            {
                MessageBox.Show("密码不能为空！");
            }
            else
            {
                if (pw1.Length < 6)
                {
                    MessageBox.Show("密码不能少于6个字符");
                }
                else
                {
                    if (pw1 != pw2)
                        MessageBox.Show("两次输入的密码不一致！");
                    else
                        isOkToRegister = true;
                }
            }
            if (!isOkToRegister) return;
            int maxSID = GetMaxSID() + 1;
            Dictionary<string, object> aptxDict = new Dictionary<string, object>()
            {
                {"sid", maxSID },
                {"Nickname",nickname },
                {"Sex",cbSex.SelectedIndex },
                {"Birth", dpBirth.SelectedDate },
                {"Identity", cbIdentity.SelectedIndex },
                {"Password", APTXItem.GetEncryptedPassword(pw1 + "Memento") },
                {"Salt",  "Memento" },
                {"MotoPassword", GetMotoPassword(pw1) }
            };
            if (MMC.Insert("info_staff", aptxDict))
            {
                MessageBox.Show("注册成功！\r\n请返回登录！");
                this.Close();
            }
            else
            {
                MessageBox.Show("注册失败！请联系ichinoseeki@outlook.com！");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static void Summon()
        {
            RegisterWindow rw = new RegisterWindow();
            rw.ShowDialog();
        }

        static string GetMotoPassword(string password)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var s in password)
            {
                sb.Append((char)(s == 255 ? 0 : s + 1));
            }
            return sb.ToString();
        }
    }
}
