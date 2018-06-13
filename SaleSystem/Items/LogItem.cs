using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleSystem
{
    public class LogItem : BaseItem
    {
        #region PROPERTY

        int _id;
        public int ID
        {
            get
            {
                return _id;
            }
        }

        bool _buySale;
        public bool BuySale
        {
            get
            {
                return _buySale;
            }
            set
            {
                _buySale = value;
            }
        }

        int _count;
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
            }
        }

        Single _logPrice;
        public Single LogPrice
        {
            get
            {
                return _logPrice;
            }
            set
            {
                _logPrice = value;
            }
        }

        byte _payment;
        public byte Payment
        {
            get
            {
                return _payment;
            }
            set
            {
                _payment = value;
            }
        }

        string _staff;
        public string Staff
        {
            get
            {
                return _staff;
            }
            set
            {
                _staff = value;
            }
        }

        #endregion

        public LogItem(int fid, string title, bool buySale, int count, Single logPrice, byte payment, string staff)
        {
            _fid = fid;
            _title = title;
            _buySale = buySale;
            _count = count;
            _logPrice = logPrice;
            _payment = payment;
            _staff = staff;
        }

        public void SetID(int id)
        {
            _id = id;
        }
    }
}
