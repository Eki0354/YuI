using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

namespace EmailElf
{
    /// <summary>
    /// EmailElfWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EmailElfWindow : Window
    {
        bool _IsRead = false;
        public EmailElfWindow()
        {
            InitializeComponent();
            double x = SystemParameters.WorkArea.Width;//得到屏幕工作区域宽度
            double y = SystemParameters.WorkArea.Height;//得到屏幕工作区域高度
            this.Left = x - this.Width - 20;
            this.Top = (y - this.Height) / 2;
            image_Elf.Source = BitmapToBitmapImage(
                Properties.Resources.email_unread);
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _IsRead = !_IsRead;
            image_Elf.Source = _IsRead ?
                BitmapToBitmapImage(Properties.Resources.email_read) :
                BitmapToBitmapImage(Properties.Resources.email_unread);

        }

        private BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png); // 坑点：格式选Bmp时，不带透明度

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
    }
}
