using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELite.ELiteItem
{
    public interface IELiteTableItem
    {
        bool InsertTo(ELiteConnection conn);
        bool DeleteFrom(ELiteConnection conn);
        bool UpdateIn(ELiteConnection conn);
        bool IsExistedIn(ELiteConnection conn);
        Dictionary<string, object> ToDictionary();
        void LoadFromDataRow(DataRow row);
    }

    public abstract class ELiteTableItem
    {
        protected abstract string _TableName { get; }
        protected abstract string _MainKey { get; }

        public virtual bool Insert<T>(ELiteConnection conn, T t) where T : IELiteTableItem
        {
            return true;
        }
    }

    #region ELiteCarTypeItem

    public struct ELiteCarTypeItem : IELiteTableItem
    {
        public int CTID { get; set; }
        public string Caption { get; set; }
        public int BasePrice { get; set; }
        public int Price { get; set; }

        public override string ToString()
        {
            return Caption;
        }
        
        #region Static

        public static ELiteCarTypeItem Empty => new ELiteCarTypeItem()
        {
            CTID = 42,
            Caption = "Bumblebee",
            BasePrice = 42,
            Price = 42
        };

        #endregion

        #region IELiteItem

        public bool InsertTo(ELiteConnection conn)
        {
            return conn.Insert("info_car_types", this.ToDictionary());
        }

        public bool DeleteFrom(ELiteConnection conn)
        {
            return conn.Delete("info_car_types", "CTID", CTID);
        }

        public bool UpdateIn(ELiteConnection conn)
        {
            return conn.Update("info_car_types", this.ToDictionary(), "CTID", CTID);
        }

        public bool IsExistedIn(ELiteConnection conn)
        {
            return conn.IsExisted("info_car_types", "CTID", CTID);
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "CTID", CTID },
                { "Caption", Caption },
                { "BasePrice", BasePrice },
                { "Price", Price }
            };
        }

        public void LoadFromDataRow(DataRow row)
        {
            this.CTID = (int)row["CTID"];
            this.Caption = (string)row["Caption"];
            this.BasePrice = (int)row["BasePrice"];
            this.Price = (int)row["Price"];
        }

        #endregion

    }

    #endregion

    #region ELiteAirportItem

    public struct ELiteAirportItem : IELiteTableItem
    {
        public int AID { get; set; }
        public string Caption { get; set; }

        public override string ToString()
        {
            return Caption;
        }

        #region Static

        public static ELiteAirportItem Empty => new ELiteAirportItem()
        {
            AID = 42,
            Caption = "简阳国际机场"
        };

        #endregion

        #region IELiteItem

        public bool InsertTo(ELiteConnection conn)
        {
            return conn.Insert("info_airport", this.ToDictionary());
        }

        public bool DeleteFrom(ELiteConnection conn)
        {
            return conn.Delete("info_airport", "AID", AID);
        }

        public bool UpdateIn(ELiteConnection conn)
        {
            return conn.Update("info_airport", this.ToDictionary(), "AID", AID);
        }

        public bool IsExistedIn(ELiteConnection conn)
        {
            return conn.IsExisted("info_airport", "AID", AID);
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "AID", AID },
                { "Caption", Caption }
            };
        }

        public void LoadFromDataRow(DataRow row)
        {
            this.AID = (int)row["AID"];
            this.Caption = (string)row["Caption"];
        }

        #endregion

    }

    #endregion

}
