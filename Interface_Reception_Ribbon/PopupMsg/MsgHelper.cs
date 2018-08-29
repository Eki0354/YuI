using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace Interface_Reception_Ribbon.PopupMsg
{
    public class MsgHelper
    {
        #region 提示消息

        /// <summary>
        /// 弹出提示消息标题为提示，按钮为确定
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowMessage(string msg, UIElement element = null)
        {
            //ShowFriendMessage(msg, "提示", MessageBoxButton.OK);

            PopupBorder pborder = new PopupBorder();
            pborder.txtMessage.Text = " " + msg + " ";

            pborder.UpdateLayout();

            Popup popUp = new Popup
            {
                Child = pborder,
                StaysOpen = false
            };
            if (element != null) popUp.PlacementTarget = element;

            Storyboard pmboard = pborder.Resources["PopupMsgStoryboard"] as Storyboard;
            
            pmboard.Completed += delegate
            {
                popUp.IsOpen = false;
            };

            pmboard.Begin();
            
            popUp.LayoutUpdated += delegate
            {
                /*popUp.Margin = new Thickness(
                        (App.Current.MainWindow.ActualWidth - pborder.ActualWidth) / 2,
                        (App.Current.MainWindow.ActualHeight - pborder.ActualHeight) / 2,
                        0,
                        0);*/

                System.Threading.Timer timer = new System.Threading.Timer(
                    (state) =>
                    {
                        popUp.Dispatcher.BeginInvoke((Action)delegate()
                        {
                            popUp.IsOpen = false;
                        });
                    }, null, 1500, 1500);
            };
            popUp.IsOpen = true;
            popUp.UpdateLayout();
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
