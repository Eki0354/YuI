using System;

namespace SaleSystem
{
    public class BaseItem
    {
        #region PROPERTY

        protected int _fid;
        public int FID
        {
            get
            {
                return _fid;
            }
        }

        protected string _uid;
        public string UID
        {
            get
            {
                return _uid;
            }
        }

        protected string _title;
        public string Title
        {
            get
            {
                return _title;
            }
        }

        protected Single _price;
        public Single Price
        {
            get
            {
                return _price;
            }
        }

        protected Single _disPrice;
        public Single DisPrice
        {
            get
            {
                return _disPrice;
            }
        }

        protected byte _type;
        public byte Type
        {
            get
            {
                return _type;
            }
        }

        #endregion

        public BaseItem()
        {

        }

        public BaseItem(int fid, string uid, string title, Single price, Single disPrice, byte type)
        {
            _fid = fid;
            _uid = uid;
            _title = title;
            _price = price;
            _disPrice = disPrice;
            _type = type;
        }
        
        public LogItem ConvertToLogItem()
        {
            return new LogItem(
                _fid,
                _title,
                false,
                1,
                _price,
                0,
                ""
                );
        }

        public CountItem ConvertToCountItem()
        {
            return new CountItem(
                _fid,
                _uid,
                _title,
                0,
                -1
                );
        }
    }
}
