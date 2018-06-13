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
using SaleSystem;
using System.Data.OleDb;
using System.Threading;
using static SaleSystem.ForceGun;

namespace Interface_Reception_Ribbon
{
    /// <summary>
    /// Page_Sale.xaml 的交互逻辑
    /// </summary>
    public partial class Page_Sale : Page
    {
        SaleFairy _sf;
        OleDbConnection _conn;

        public Page_Sale()
        {
            InitializeComponent();
            OpenOleDb();
            _sf = new SaleFairy(_conn);
            lable_leftItemCount.DataContext = _sf;
            lable_soldItemCount.DataContext = _sf;
            dg_sale.DataContext = _sf;
            _listener = new ScannerHook();
            _listener.ScanerEvent += Listener_ScannerEvent;
            Scaner_Load(null, null);
        }

       
        #region OLEDB

        private void OpenOleDb()
        {
            OleDbConnectionStringBuilder oleStr = new OleDbConnectionStringBuilder();
            oleStr.Provider = "Microsoft.Jet.OleDB.4.0";
            oleStr.PersistSecurityInfo = true;
            oleStr.DataSource = Environment.CurrentDirectory + "\\database\\sale.mdb";
            oleStr.Add("Jet OLEDB:DataBase Password", "Eki20150613");
            _conn = new OleDbConnection();
            _conn.ConnectionString = oleStr.ConnectionString;
            Console.WriteLine(_conn.ConnectionString);
            _conn.Open();
        }

        private void CloseOleDb()
        {
            _conn.Close();
        }

        #endregion

        #region SCANNER

        ScannerHook _listener;

        private void Listener_ScannerEvent(ScannerHook.ScanerCodes codes)
        {
            string uid = codes.Result;
            BaseItem bi = _sf.GetBaseItem(uid);
            if (bi == null)
            {
                if (!IsPopupOpen(pu_addItem))
                {
                    ShowPopupDialog(pu_tip_noItem, true);
                }
            }
            else
            {
                _sf.WriteLogItem(bi.ConvertToLogItem(), DateTime.Now);
            }
        }

        private void Scaner_Load(object sender, System.EventArgs e)
        {
            _listener.Start();
        }

        private void Scaner_FormClosing(object sender, EventArgs e)
        {
            _listener.Stop();
        }

        #endregion

        #region POPUP

        #region SHARED

        public bool IsPopupOpen(System.Windows.Controls.Primitives.Popup popup)
        {
            return popup.IsOpen;
        }

        private void ShowPopupDialog(System.Windows.Controls.Primitives.Popup popup, bool isDialog)
        {
            popup.IsOpen = true;
            this.IsEnabled = !isDialog;
            Application.Current.MainWindow.IsEnabled = !isDialog;
            popup.Focus();
        }

        private void HidePopupDialog(System.Windows.Controls.Primitives.Popup popup)
        {
            popup.IsOpen = false;
            this.IsEnabled = true;
            Application.Current.MainWindow.IsEnabled = true;
        }

        #endregion

        #region NOITEM
        
        public void ShowPopup_NoItem(bool isDialog)
        {
            ShowPopupDialog(pu_tip_noItem, isDialog);
        }

        public void HidePopup_NoItem()
        {
            HidePopupDialog(pu_tip_noItem);
        }

        private void b_sale_noItem_add_Click(object sender, RoutedEventArgs e)
        {
            HidePopupDialog(pu_tip_noItem);
            ShowPopupDialog(pu_addItem, true);
        }

        private void b_sale_noItem_cancel_Click(object sender, RoutedEventArgs e)
        {
            HidePopupDialog(pu_tip_noItem);
        }

        #endregion

        #region ADDITEM

        public void ShowPopup_AddItem(bool isDialog)
        {
            ShowPopupDialog(pu_addItem, isDialog);
            tb_sale_addItem_uid.Focus();
        }

        public void HidePopup_AddItem()
        {
            HidePopupDialog(pu_addItem);
        }

        private void b_sale_addItem_add_Click(object sender, RoutedEventArgs e)
        {
            string uid = tb_sale_addItem_uid.Text;
            if (uid == "") { lb_sale_tips.Content = "编号不能为空！"; return; }
            string title = tb_sale_addItem_title.Text;
            if (title == "") { lb_sale_tips.Content = "名称不能为空！"; return; }
            Single price;
            try
            {
                price = Convert.ToSingle(tb_sale_addItem_price.Text);
            }
            catch
            {
                lb_sale_tips.Content = "价格不能为空！"; return;
            }
            Single disPrice;
            try
            {
                disPrice = Convert.ToSingle(tb_sale_addItem_disPrice.Text);
            }
            catch
            {
                lb_sale_tips.Content = "请输入正确的折扣价格！"; return;
            }
            BaseItem bi = new BaseItem(
                -1,
                uid,
                title,
                price,
                disPrice,
                (byte)cb_sale_addItem_countable.SelectedIndex
                );
            HidePopupDialog(pu_addItem);
            if (_sf.AddItem(bi))
            {
                lb_sale_tips.Content = "成功添加物品！";
                _sf.Refresh();
                //添加成功后清除界面元素的内容
                tb_sale_addItem_uid.Clear();
                tb_sale_addItem_title.Clear();
                tb_sale_addItem_price.Clear();
                tb_sale_addItem_disPrice.Clear();
                cb_sale_addItem_countable.SelectedIndex = 0;
            }
            else
            {
                lb_sale_tips.Content = "添加物品失败！可能已存在相同编号的物品，请检查数据库记录。";
            }
        }

        private void b_sale_addItem_cancel_Click(object sender, RoutedEventArgs e)
        {
            lb_sale_tips.Content = "已取消添加物品！";
            HidePopupDialog(pu_addItem);
        }
        
        #endregion

        #endregion
        
    }
}