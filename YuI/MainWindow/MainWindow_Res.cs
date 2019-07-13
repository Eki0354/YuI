using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Ribbon;
using System.Windows.Threading;
using MementoConnection;
using YuI.EControls;
using IOExtension;
using MMC = MementoConnection.MMConnection;
using System.Data;

namespace YuI
{
    #region 员工名字排序-弃用

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

    #endregion

    public partial class MainWindow
    {
        Page_Reservation _pageRes;
        public Ran.APTXItem MementoAPTX =>
            MMC.LoggedInStaffDataRow is DataRow row ? Ran.APTXItem.FromDataRow(row) : null;

        // Using a DependencyProperty as the backing store for StaffList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StaffListProperty =
            DependencyProperty.Register("StaffList",
                typeof(List<string>),
                typeof(MainWindow), 
                new PropertyMetadata(null));
        
    }
}
