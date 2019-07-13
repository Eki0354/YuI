using FFElf;
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
using System.Windows.Shapes;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Net.Mail;
using System.IO;

namespace YuI
{
    /// <summary>
    /// EmailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EmailWindow : Window
    {
        public EmailWindow()
        {
            InitializeComponent();
        }

        public static void Summon(OutlookEmail email)
        {
            var ew = new EmailWindow();
            ew.AddressTextBox.Text = email.Address;
            ew.ThemeTextBox.Text = email.Theme;
            ew.BodyRichTextBox.Document.Blocks.Clear();
            ew.BodyRichTextBox.AppendText(email.Body);
            ew.ShowDialog();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string _sender = "mrspandahostel@hotmail.com";// "ichinoseeki@outlook.com";
            SmtpClient client = new SmtpClient("smtp-mail.outlook.com")
            {
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(
                    _sender, "Panda2018**")// "ms#425459#");
            };
            try
            {
                //"911486667@qq.com");
                var mail = new MailMessage(_sender.Trim(), this.AddressTextBox.Text.Trim())
                {
                    Subject = this.ThemeTextBox.Text,
                    IsBodyHtml = true,
                    Body = BodyRichTextBox.Document.ToString()
                };
                client.Send(mail);
            }
            catch
            {

            }
        }

        public static string ReadSignature()
        {
            string sign = "";
            using (var fs = new FileStream(IOExtension.MementoPath.SignaturePath,
                FileMode.OpenOrCreate))
            {
                using (var sr = new StreamReader(fs))
                {
                    sign = sr.ReadToEnd();
                }
            }
            return sign;
        }
    }
}
