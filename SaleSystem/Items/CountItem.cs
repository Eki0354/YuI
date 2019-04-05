using System;
using System.Collections.Generic;

namespace SaleSystem
{
    public class CountItem
    {
        #region PROPERTY

        int _fid;
        public int FID
        {
            get
            {
                return _fid;
            }
        }

        private string _uid;
        public string UID
        {
            get
            {
                return _uid;
            }
        }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
        }

        byte _type;
        public byte Type
        {
            get
            {
                return _type;
            }
        }

        private int _leftCount;
        public int LeftCount
        {
            get
            {
                return LeftCount;
            }
        }

        private int _buyCount;
        public int BuyCount
        {
            get
            {
                return _buyCount;
            }
            set
            {
                _buyCount = value;
            }
        }

        private int _saleCount;
        public int SaleCount
        {
            get
            {
                return _saleCount;
            }
            set
            {
                _saleCount = value;
            }
        }

        List<LogItem> _logs = new List<LogItem>();

        #endregion

        public CountItem(int fid, string uid, string title, byte type, int leftCount = 0, int buyCount = 0, int saleCount = 0)
        {
            _uid = uid;
            _title = title;
            _type = type;
            _leftCount = leftCount;
            _buyCount = buyCount;
            _saleCount = saleCount;
        }

        #region OPERATION

        public LogItem GetLog(int id)
        {
            if (_logs.Count == 0) { return null; }
            return _logs.Find(l => l.ID == id);
        }

        public void AddLog(LogItem li)
        {
            if(_title!=li.Title) { return; }
            _logs.Add(li);
        }

        public bool DeleteLog(LogItem li)
        {
            return _logs.Remove(li);
        }

        public void AddLogs(List<LogItem> liList)
        {
            liList.ForEach(li => AddLog(li));
        }

        #endregion

        #region OPERATOR

        private void INC()
        {
            _buyCount++;
        }

        private void DEC()
        {
            _saleCount++;
        }

        private void Add(int v)
        {
            _buyCount += v;
        }

        private void Minus(int v)
        {
            _saleCount += v;
        }

        public static CountItem operator ++(CountItem ci)
        {
            ci._buyCount++;
            return ci;
        }

        public static CountItem operator --(CountItem ci)
        {
            ci._saleCount++;
            return ci;
        }

        public static CountItem operator +(CountItem ci, int v)
        {
            ci._buyCount += v;
            return ci;
        }

        public static CountItem operator -(CountItem ci, int v)
        {
            ci._saleCount += v;
            return ci;
        }

        public static bool operator ==(CountItem ci1, CountItem ci2)
        {
            return ci1._uid == ci2._uid;
        }

        public static bool operator !=(CountItem ci1, CountItem ci2)
        {
            return ci1._uid != ci2._uid;
        }

        #endregion

        #region COUNT

        public string LeftCountString
        {
            get
            {
                return _title + "=" + _leftCount.ToString();
            }
        }

        public string CurrentCountString
        {
            get
            {
                return _title + "=" + (_leftCount + _buyCount - _saleCount).ToString();
            }
        }

        public string SaleCountString
        {
            get
            {
                return _title + "=" + _saleCount.ToString();
            }
        }

        public int LogCount
        {
            get
            {
                return _logs.Count;
            }
        }

        #endregion

        #region SUM

        public Single Sum()
        {
            Single sum = 0;
            _logs.ForEach(log => sum += log.LogPrice * log.Count);
            return sum;
        }

        //求取当前记录项的指定支付方式的销售总额
        public Single Sum(int payment)
        {
            Single sum = 0;
            _logs.ForEach(log =>
            {
                if (log.Payment == payment)
                {
                    sum += log.LogPrice * log.Count;
                }
            });
            return sum;
        }

        //求取当前记录项的销售现金总额
        public Single Sum_Cash
        {
            get
            {
                return Sum(0);
            }
        }

        #endregion
    }
}
