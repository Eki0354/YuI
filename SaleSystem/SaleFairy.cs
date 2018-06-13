using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using Interface_Reception_Ribbon.Sale;
using System.ComponentModel;

namespace SaleSystem
{
    public class SaleFairy : INotifyPropertyChanged
    {
        #region PROPERTY

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        OleDbConnection _conn;

        DataTable _cacheLogDataTable;
        public DataTable CacheLogDataTable
        {
            get
            {
                return _cacheLogDataTable;
            }
            set
            {
                _cacheLogDataTable = _GetCacheLogItemDataTable();
                OnPropertyChanged("CacheLogDataTable");
            }
        }

        #endregion

        public SaleFairy(OleDbConnection conn)
        {
            _conn = conn;
            Initialize();
        }

        #region BASIC
        
        public void Refresh()
        {
            ReadCacheCountItems(true);
            ReadCacheLogs(true);
            CacheLogDataTable = _GetCacheLogItemDataTable();
            LeftCountString = _LeftCountString();
            CurrentCountString = _CurrentCountString();
            SaleCountString = _SaleCountString();
        }

        private bool RunSql(OleDbCommand comm)
        {
            try
            {
                comm.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Initialize()
        {
            InitializeBaseItemSystem();
            InitializeCountSystem();
        }

        #endregion

        #region BASEITEMSYSTEM
        
        List<BaseItem> _baseItems = new List<BaseItem>();

        public bool AddItem(BaseItem bi)
        {
            OleDbCommand comm = new OleDbCommand();
            comm.Connection = _conn;
            comm.CommandText = "Insert into info_items (UID, Title, Price, DisPrice, Type) " +
                "values (@uid, @title, @price, @disPrice, @type)";
            comm.Parameters.AddWithValue("@uid", bi.UID);
            comm.Parameters.AddWithValue("@title", bi.Title);
            comm.Parameters.AddWithValue("@price", bi.Price);
            comm.Parameters.AddWithValue("@disPrice", bi.DisPrice);
            comm.Parameters.AddWithValue("@type", bi.Type);
            return RunSql(comm);
        }

        public bool DeleteItem(int id = -1, string uid = "", string title = "")
        {
            OleDbCommand comm = new OleDbCommand();
            comm.Connection = _conn;
            comm.CommandText = "Delete from info_items where";
            if (id != -1)
            {
                comm.CommandText += " ID=" + id;
                return RunSql(comm);
            }
            if (uid != "")
            {
                comm.CommandText += " UID=" + uid;
                return RunSql(comm);
            }
            if (title != "")
            {
                comm.CommandText += " Title=" + title;
                return RunSql(comm);
            }
            return false;
        }

        public void DeleteItem(BaseItem bi)
        {
            DeleteItem(bi.FID, bi.UID, bi.Title);
        }

        public bool ClearItems()
        {
            OleDbCommand comm = new OleDbCommand();
            comm.Connection = _conn;
            comm.CommandText = "Delete * from info_items";
            return RunSql(comm);
        }

        public int GetFIDInBaseItem(string title)
        {
            BaseItem bi = _baseItems.Find(x => x.Title == title);
            if (bi == null) { return -1; }
            return bi.FID;
        }

        public string GetUIDInBaseItem(string title)
        {
            BaseItem bi = _baseItems.Find(x => x.Title == title);
            if (bi == null)
                return null;
            return bi.UID;
        }

        public BaseItem GetBaseItem(string uid="", string title = "")
        {
            if (uid == "" && title == "")
                return null;
            BaseItem bi = _baseItems.Find(x => x.UID == uid);
            if (bi != null)
                return bi;
            bi = _baseItems.Find(x => x.Title == title);
            if (bi != null)
                return bi;
            return null;
        }

        public List<BaseItem> GetAllItems(bool isRefresh = false)
        {
            //参数为否时，不刷新列表；否则获取所有基本物品列表的更新
            if(!isRefresh) { return _baseItems; }
            _baseItems.Clear();
            OleDbCommand comm = new OleDbCommand();
            comm.Connection = _conn;
            comm.CommandText = "Select * from info_items";//ID, UID, Title, Price, DisPrice
            OleDbDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                BaseItem bi = ReadItemTo(reader);
                _baseItems.Add(bi);
            }
            return _baseItems;
        }

        private BaseItem ReadItemTo(OleDbDataReader reader)
        {
            return new BaseItem(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetFloat(3),
                reader.GetFloat(4),
                reader.GetByte(5)
                );
        }

        private string GetValueInBaseItem(string uid, string key)
        {
            if (key == "") { return null; }
            OleDbCommand comm = new OleDbCommand();
            comm.Connection = _conn;
            comm.CommandText = "Select " + key + " from info_items where UID=" + uid;
            OleDbDataReader reader = comm.ExecuteReader();
            if(!reader.HasRows) { return null; }
            reader.Read();
            return reader.GetValue(0).ToString();
        }

        public bool ContainsInBaseItem(string uid)
        {
            return _baseItems.Find(bi => bi.UID == uid) != null;
        }

        private void InitializeBaseItemSystem()
        {
            GetAllItems(true);
        }

        #endregion

        #region COUNTSYSTEM
        
        #region PROPERTY
        
        string _leftCountString;
        public string LeftCountString
        {
            get
            {
                return _leftCountString;
            }
            set
            {
                _leftCountString = _LeftCountString();
                OnPropertyChanged("LeftCountString");
            }
        }

        string _currentCountString;
        public string CurrentCountString
        {
            get
            {
                return _currentCountString;
            }
            set
            {
                _currentCountString = _CurrentCountString();
                OnPropertyChanged("CurrentCountString");
            }
        }

        string _saleCountString;
        public string SaleCountString
        {
            get
            {
                return _saleCountString;
            }
            set
            {
                _saleCountString = _SaleCountString();
                OnPropertyChanged("SaleCountString");
            }
        }

        #endregion

        #region COUNTITEMS

        private List<CountItem> _countItems = new List<CountItem>();

        public List<CountItem> ReadCacheCountItems(bool isRefresh = false)
        {
            if(!isRefresh) { return _countItems; }
            _countItems.Clear();
            //以数据库内所有记录项填充空的记录列表
            _baseItems.ForEach(bi => _countItems.Add(bi.ConvertToCountItem()));
            _logItems.ForEach(li =>
            {
                //查找当前记录项列表中相同名称的项，若不存在，则创建新记录项；若存在则添加此记录
                CountItem ci = _countItems.Find(x => x.Title == li.Title);
                if (ci == null)
                {
                    ci = new CountItem(
                        li.FID,
                        li.UID,
                        li.Title,
                        li.Type
                        );
                }
                ci.AddLog(li);
            });
            return _countItems;
        }

        public CountItem ReadCountItem(int fid)
        {
            if(_countItems.Count == 0) { return null; }
            return _countItems.Find(ci => ci.FID == fid);
        }

        public void WriteCountItem(CountItem ci, DateTime recorded)
        {
            OleDbCommand comm = new OleDbCommand();
            comm.Connection = _conn;
            comm.CommandText = "Insert into log_count (FID, LeftCount, BuyCount, SaleCount, Recorded) " +
                "values (@fid, @leftCount, @buyCount, @saleCount, @recorded)";
            comm.Parameters.AddWithValue("@fid", ci.FID);
            comm.Parameters.AddWithValue("@leftCount", ci.LeftCount);
            comm.Parameters.AddWithValue("@buyCount", ci.BuyCount);
            comm.Parameters.AddWithValue("@saleCount", ci.SaleCount);
            comm.Parameters.AddWithValue("@recorded", recorded);
            comm.ExecuteNonQuery();
        }

        public void WriteCountItems(List<CountItem> ciList, DateTime recorded)
        {
            ciList.ForEach(ci => WriteCountItem(ci, recorded));
        }

        public void WriteAllCountItems(DateTime recorded)
        {
            WriteCountItems(_countItems, recorded);
        }

        #endregion
        
        #region CHANGECOUNT

        public void Add(string uid, int v)
        {
            CountItem ci = _countItems.Find(x => x.UID == uid);
            ci += v;
        }

        public void Minus(string uid, int v)
        {
            CountItem ci = _countItems.Find(x => x.UID == uid);
            ci -= v;
        }

        #endregion

        #region SUM

        //求当前吧台销售现金总额
        private Single _BarSumSale()
        {
            Single sub = 0;
            _countItems.ForEach(x =>
            {
                if (x.Type == 0)
                {
                    sub += x.Sum_Cash;
                }
            });
            return sub;
        }

        //求当前前台非房费销售现金总额
        private Single _NonBarSumSale()
        {
            Single sub = 0;
            _countItems.ForEach(x =>
            {
                if (x.Type != 0)
                {
                    sub += x.Sum_Cash;
                }
            });
            return sub;
        }

        #endregion

        #region COUNTRETURN

        private string _LeftCountString()
        {
            List<string> sL = new List<string>();
            _countItems.ForEach(x => sL.Add(x.LeftCountString));
            return String.Join("\n", sL.ToArray());
        }

        private string _CurrentCountString()
        {
            List<string> sL = new List<string>();
            _countItems.ForEach(x => sL.Add(x.CurrentCountString));
            return String.Join("\n", sL.ToArray());
        }

        private string _SaleCountString()
        {
            List<string> sL = new List<string>();
            _countItems.ForEach(x => sL.Add(x.SaleCountString));
            return String.Join("\n", sL.ToArray());
        }

        #endregion

        #region LOGS

        private DataTable _preCacheLogItemDataTable;
        private List<LogItem> _logItems = new List<LogItem>();

        public List<LogItem> ReadCacheLogs(bool isRefresh = false)
        {
            //参数为否时，返回当前未记账记录项列表；为是则获取更新
            if (!isRefresh) { return _logItems; }
            _logItems.Clear();
            OleDbCommand comm = new OleDbCommand();
            comm.Connection = _conn;
            comm.CommandText = "Select info_items.UID, info_items.Type, log_items.FID, info_items.Title, " +
                "log_items.BuySale, log_items.Count, log_items.LogPrice, log_items.Payment" +
                "from log_items, info_items where log_items.FID = info_items.ID AND log_items.Staff=''";
            OleDbDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                LogItem li = new LogItem(
                    reader.GetInt32(2),
                    reader.GetString(3),
                    reader.GetBoolean(4),
                    reader.GetInt32(5),
                    reader.GetFloat(6),
                    reader.GetByte(7),
                    ""
                    );
                _logItems.Add(li);
            }
            return _logItems;
        }

        public LogItem ReadLogItem(int id)
        {
            if (_countItems.Count == 0) { return null; }
            LogItem li = null;
            _countItems.ForEach(ci =>
            {
                li = ci.GetLog(id);
                if (li != null) { return; }
            });
            return li;
        }

        private DataTable _GetCacheLogItemDataTable()
        {
            DataTable dT = _preCacheLogItemDataTable.Clone();
            _logItems.ForEach(li =>
            {
                DataRow dR = dT.NewRow();
                dR["ID"] = li.ID;
                dR["Title"] = li.Title;
                dR["BuySale"] = li.BuySale;
                dR["Count"] = li.Count;
                dR["Price"] = li.LogPrice;
            });
            return dT;
        }

        private DataColumn NewDataColumn(string header, Type dataType, string expression = "")
        {
            DataColumn dC = new DataColumn();
            dC.ColumnName = header;
            dC.DataType = dataType.GetType();
            if (expression != "") { dC.Expression = expression; }
            return dC;
        }

        public bool WriteLogItem(LogItem li, DateTime recorded)
        {
            OleDbCommand comm = new OleDbCommand();
            comm.Connection = _conn;
            comm.CommandText = "Insert into log_items (FID, BuySale, Count, LogPrice, Payment, Staff, Recorded) " +
                "values (@fid, @buySale, @count, @logPrice, @payment, @staff, @recorded)";
            comm.Parameters.AddWithValue("@fid", li.FID);
            comm.Parameters.AddWithValue("@buySale", li.BuySale);
            comm.Parameters.AddWithValue("@count", li.Count);
            comm.Parameters.AddWithValue("@logPrice", li.LogPrice);
            comm.Parameters.AddWithValue("@payment", li.Payment);
            comm.Parameters.AddWithValue("@staff", li.Staff);
            comm.Parameters.AddWithValue("@recorded", recorded);
            return RunSql(comm);
        }

        public void WriteLogItems(List<LogItem> liList, DateTime recorded)
        {
            liList.ForEach(li => WriteLogItem(li, recorded));
        }

        public bool HandleCacheLogItem(LogItem li)
        {
            OleDbCommand comm = new OleDbCommand();
            comm.Connection = _conn;
            comm.CommandText = "Update log_items set Staff=" + li.Staff + " where ID=" + li.ID;
            return RunSql(comm);
        }

        public void HandleCacheLogItems(List<LogItem> liList)
        {
            liList.ForEach(li => HandleCacheLogItem(li));
        }

        #endregion

        #region INITIALIZE

        private void InitializeCountSystem()
        {
            DataTable dT = new DataTable();
            dT.Columns.Add(NewDataColumn("ID", typeof(int)));
            dT.Columns.Add(NewDataColumn("Title", typeof(string)));
            dT.Columns.Add(NewDataColumn("BuySale", typeof(bool)));
            dT.Columns.Add(NewDataColumn("Count", typeof(int)));
            dT.Columns.Add(NewDataColumn("Price", typeof(Single)));
            dT.Columns.Add(NewDataColumn("Sum", typeof(Single), "Count * Price"));
            _preCacheLogItemDataTable = dT;
            ReadCacheCountItems(true);
        }

        #endregion

        #endregion

    }
}
