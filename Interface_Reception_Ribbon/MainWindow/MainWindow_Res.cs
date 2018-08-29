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
using System.Data;
using System.Data.SQLite;
using System.Windows.Controls.Ribbon;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using ELite;
using EkiXmlDocument;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using ELite.Reservation;

namespace Interface_Reception_Ribbon
{
    public partial class MainWindow
    {
        Page_Reservation _pageRes;
        string _StaffName => rcb_staff.SelectionBoxItem as string;

        #region Initialize

        /// <summary> 负责更新RibbonTabs中订单界面的控件初始化 </summary>
        private void InitializeControls_ResGroup()
        {
            InitializeStaff();
            InitializeEmailTemplets();
        }

        /// <summary> 初始化员工列表 </summary>
        private void InitializeStaff()
        {
            List<string> staff = _pageRes.GetStaffList;
            staff.ForEach(s =>
            {
                RibbonGalleryItem item = new RibbonGalleryItem();
                item.Content = s;
                item.Width = 80;
                rgc_staff.Items.Add(item);
            });
        }

        /// <summary> 初始化邮件模板列表 </summary>
        private void InitializeEmailTemplets()
        {
            List<string> emailTemplets = _pageRes.ReadEmailTempletList();
            emailTemplets.ForEach(et =>
            {
                RibbonButton rb = new RibbonButton();
                rb.Label = et;
                rb.Click += _pageRes.SetEmailTempletText;
                rsb_emailTemplets.Items.Add(rb);
            });
        }

        #endregion

        private void rb_copy_Click(object sender, RoutedEventArgs e)
        {
            string comments = _pageRes.CommentString;
            string staff = string.Empty;
            object staffObj = rcb_staff.SelectionBoxItem;
            string cellString = _pageRes.CellString;
            if (staffObj is null || string.IsNullOrEmpty(cellString))
            {
                PopupMsg.MsgHelper.ShowMessage("复制失败！", rb_copy);
                return;
            }
            staff = staffObj.ToString();
            Clipboard.SetText(cellString + ";;;" + comments + "\r\n" + _StaffName +
                " " + DateTime.Now.ToString("yyyy/MM/dd"));
            PopupMsg.MsgHelper.ShowMessage("复制成功！", rb_copy);
        }

        /// <summary> 为复制确认预订的邮件内容的RibbonMenuItem提供点击事件，
        /// 以Tag属性作为标记，具体过程调用CopyEmailReplies方法实现。 </summary>
        private void EmailReplyRibbonMenuItems_Clicked(object sender, RoutedEventArgs e)
        {
            RibbonButton rb = sender as RibbonButton;
            if (rb == null) return;
            CopyEmailReplies(rb.Tag.ToString());
        }

        /// <summary> 方法：复制确认预订的邮件内容，包括邮箱地址、邮件主题、邮件正文。 </summary>
        private void CopyEmailReplies(string tag)
        {
            switch (tag)
            {
                case "0":
                    string emailAddress = _pageRes.EmailAddress;
                    if (string.IsNullOrEmpty(emailAddress))
                    {
                        PopupMsg.MsgHelper.ShowMessage("失败！邮箱地址不能为空！", rsb_emailReply);
                    }
                    else
                    {
                        Clipboard.SetText(_pageRes.EmailAddress);
                        PopupMsg.MsgHelper.ShowMessage("已成功复制邮箱地址！", rsb_emailReply);
                    }
                    break;
                case "1":
                    Clipboard.SetText(_pageRes.EmailTheme.Replace("StaffName", _StaffName));
                    PopupMsg.MsgHelper.ShowMessage("已成功复制邮件主题！", rsb_emailReply);
                    break;
                case "2":
                    try
                    {
                        Clipboard.SetText(_pageRes.EmailText(_StaffName));
                        PopupMsg.MsgHelper.ShowMessage("已成功复制邮件正文！", rsb_emailReply);
                    }
                    catch
                    {
                        PopupMsg.MsgHelper.ShowMessage("生成邮件正文失败！", rsb_emailReply);
                    }
                    break;
                default:
                    PopupMsg.MsgHelper.ShowMessage("无法识别的指令！", rsb_emailReply);
                    break;
            }
        }

        private void GetHWRes(object sender, RoutedEventArgs e)
        {
            _pageRes.GetHWRes();
        }

        private void FindRes(object sender, RoutedEventArgs e)
        {
            _pageRes.FindOrder();
        }

        private void DeleteRes(object sender, RoutedEventArgs e)
        {
            _pageRes.DeleteSelectedResItem();
        }
    }
}
