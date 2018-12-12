using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EControlsLibrary
{
    /// <summary>
    /// PopupBorder.xaml 的交互逻辑
    /// </summary>
    internal partial class MessagePopup : Popup
    {
        internal MessagePopup()
        {
            InitializeComponent();
        }
        
    }

    public class EMsgBox
    {

        #region 提示消息

        /// <summary>
        /// 弹出提示消息标题为提示，按钮为确定
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowMessage(string msg, UIElement element = null)
        {
            //ShowFriendMessage(msg, "提示", MessageBoxButton.OK);

            MessagePopup popup = new MessagePopup()
            {
                StaysOpen = false
            };

            popup.txtMessage.Text = " " + msg + " ";

            popup.UpdateLayout();

            if (element != null) popup.PlacementTarget = element;

            Storyboard pmboard = popup.Resources["PopupMsgStoryboard"] as Storyboard;

            pmboard.Completed += delegate
            {
                popup.IsOpen = false;
            };

            pmboard.Begin();

            popup.LayoutUpdated += delegate
            {
                /*popUp.Margin = new Thickness(
                        (App.Current.MainWindow.ActualWidth - pborder.ActualWidth) / 2,
                        (App.Current.MainWindow.ActualHeight - pborder.ActualHeight) / 2,
                        0,
                        0);*/

                System.Threading.Timer timer = new System.Threading.Timer(
                    (state) =>
                    {
                        popup.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            popup.IsOpen = false;
                        });
                    }, null, 1500, 1500);
            };
            popup.IsOpen = true;
            popup.UpdateLayout();
        }

        /// <summary>
        /// 弹出提示消息按钮为确定
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowMessage(string msg, string title)
        {
            ShowMessage(msg, title, MessageBoxButton.OK);
        }

        /// <summary>
        /// 弹出提示消息
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowMessage(string msg, string title, MessageBoxButton buttons)
        {
            MessageBox.Show(msg, title, buttons);
        }

        #endregion

    }
}
