using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Ribbon;
using System.Windows.Threading;
using MementoConnection;
using YuI.EControls;
using Feb;
using IOExtension;
using MMC = MementoConnection.MMConnection;
using System.Data;

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
        public Ran.APTXItem MementoAPTX =>
            MMC.LoggedInStaffDataRow is DataRow row ? Ran.APTXItem.FromDataRow(row) : null;
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
            string cellString = _pageRes.CellString;
            Clipboard.SetText(cellString + ";;;" + comments + "\r\n\r\n" + this.MementoAPTX.Nickname +
                " " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            Bubble.Popup("复制详情成功！", "");
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
