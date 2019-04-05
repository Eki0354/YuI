using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Ribbon;
using System.Windows.Threading;
using MementoConnection;
using YuI.EControls;
using Feb;
using IOExtension;

namespace YuI
{
    public static class MainWindowExtention
    {
        public static List<T> GetDisruptedItems<T>(this List<T> list)
        {
            //生成一个新数组：用于在之上计算和返回
            List<T> temp = new List<T>();
            list.ForEach(item => temp.Add(item));
            //打乱数组中元素顺序
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < temp.Count; i++)
            {
                int x, y; T t;
                x = rand.Next(0, temp.Count);
                do
                {
                    y = rand.Next(0, temp.Count);
                } while (y == x);
                t = temp[x];
                temp[x] = temp[y];
                temp[y] = t;
            }
            return temp;
        }

        public static List<string> GetDisruptedStaff(this List<string> list)
        {
            list = list.GetDisruptedItems();
            if (!list.Contains("Eki")) list.Add("Eki");
            if (!list.Contains("Mori")) list.Add("Mori");
            int eI = list.IndexOf("Eki");
            int mI = list.IndexOf("Mori");
            if (Math.Abs(eI - mI) != 1)
            {
                if(eI<mI)
                {
                    list[eI] = list[mI - 1];
                    list[mI - 1] = "Eki";
                }
                else
                {
                    list[mI] = list[eI - 1];
                    list[eI - 1] = "Mori";
                }
            }
            return list;
        }
    }

    public partial class MainWindow
    {
        Page_Reservation _pageRes;
        string _StaffName => rcb_staff.SelectionBoxItem as string;
        DispatcherTimer _StaffTimer;

        public List<string> StaffList
        {
            get { return (List<string>)GetValue(StaffListProperty); }
            set { SetValue(StaffListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StaffList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StaffListProperty =
            DependencyProperty.Register("StaffList",
                typeof(List<string>),
                typeof(MainWindow), 
                new PropertyMetadata(null));

        #region Initialize

        /// <summary> 负责更新RibbonTabs中订单界面的控件初始化 </summary>
        private void InitializeControls_ResGroup()
        {
            InitializeEmailTemplates();
            InitStaff(null, null);
            _StaffTimer = new DispatcherTimer();
            _StaffTimer.Tick += new EventHandler(InitStaff);
            _StaffTimer.Interval = new TimeSpan(0, 5, 0);
            _StaffTimer.Start();
        }

        /// <summary> 初始化员工列表 </summary>
        private void InitStaff(object sender, EventArgs e)
        {
            StaffList = _pageRes.StaffList.GetDisruptedStaff();
        }
        
        /// <summary> 初始化邮件模板列表 </summary>
        private void InitializeEmailTemplates()
        {
            List<string> emailTemplets = _pageRes.ReadEmailTempletList();
            emailTemplets.ForEach(et =>
            {
                RibbonButton rb = new RibbonButton
                {
                    Label = et
                };
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
                Bubble.Popup("复制详情失败！", "员工项为必选！", BubbleStyle.Error);
                return;
            }
            staff = staffObj.ToString();
            Clipboard.SetText(cellString + ";;;" + comments + "\r\n\r\n" + _StaffName +
                " " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            Bubble.Popup("复制详情成功！", "");
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
                        Bubble.Popup("失败！", "邮箱地址不能为空！", BubbleStyle.Error);
                    }
                    else
                    {
                        Clipboard.SetText(_pageRes.EmailAddress);
                        Bubble.Popup("成功！", "已复制邮箱地址");
                    }
                    break;
                case "1":
                    Clipboard.SetText(_pageRes.EmailThemeTemplet.Replace("StaffName", _StaffName));
                    Bubble.Popup("成功！", "已复制邮件主题");
                    break;
                case "2":
                    Clipboard.SetText(_pageRes.BuildRoomDetails(_StaffName));
                    try
                    {
                        
                        Bubble.Popup("成功！", "已复制邮件正文");
                    }
                    catch
                    {
                        Bubble.Popup("失败！", "此来源网站的订单无法自动生成确认邮件", BubbleStyle.Error);
                    }
                    break;
                default:
                    Bubble.Popup("失败！", "无法识别的指令!", BubbleStyle.Warning);
                    break;
            }
        }

        private void GetHWRes(object sender, RoutedEventArgs e)
        {
            _pageRes.GetHWRes();
        }

        private void FindRes(object sender, RoutedEventArgs e)
        {
            _pageRes.FindRes();
        }

        private void UpdateConfig(object sender, RoutedEventArgs e)
        {
            _pageRes.UpdateResConfig();
        }

        private void OpenConfig(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(MementoPath.ResConfigPath);
        }

    }
}
