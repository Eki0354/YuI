using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MementoConnection;

namespace YuI.EControls
{
    /// <summary>
    /// MonthSelector.xaml 的交互逻辑
    /// </summary>
    public partial class YearMonthSelector : UserControl
    {
        #region Properties

        public int StartYear
        {
            get { return (int)GetValue(StartYearProperty); }
            set
            {
                SetValue(StartYearProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for StartYear.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartYearProperty =
            DependencyProperty.Register("StartYear",
                typeof(int),
                typeof(YearMonthSelector),
                new PropertyMetadata(2014));
        
        public int EndYear
        {
            get { return (int)GetValue(EndYearProperty); }
            set
            {
                SetValue(EndYearProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for EndYear.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndYearProperty =
            DependencyProperty.Register("EndYear",
                typeof(int),
                typeof(YearMonthSelector),
                new PropertyMetadata(2025));

        #endregion

        private List<int> YearList
        {
            get
            {
                List<int> years = new List<int>();
                if (StartYear < EndYear) return years;
                for (int year = StartYear; year < EndYear + 1; year++)
                {
                    years.Add(year);
                }
                return years;
            }
        }

        public int Year => cb_year.SelectedIndex < 0 ?
            ELiteProperties.BirthdayOfMori.Year : (int)cb_year.SelectedItem;

        public int Month => lb_month.SelectedIndex + 1;

        public YearMonthSelector()
        {
            InitializeComponent();
            SelectCurrentYear();
        }

        private void SelectCurrentYear()
        {
            int year = DateTime.Now.Year;
            if (year < 2014 || year > 2025)
            {
                System.Windows.Forms.MessageBox.Show("系统时间有误！无法运行！");
                Environment.Exit(0);
            }
            cb_year.SelectedItem = year;
        }
    }
}
