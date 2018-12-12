using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELite.ELiteItem
{

    #region ELiteBedLogItem

    public struct ELiteBedLogItem : IELiteTableItem
    {
        public int ID { get; set; }
        public int BUID { get; set; }
        public string ResNumber { get; set; }
        public int UID { get; set; }
        public string FullName { get; set; }
        public DateTime ReservedDate { get; set; }
        public bool IsPaid { get; set; }
        public float Deposit { get; set; }
        public float Price { get; set; }
        public int Discount { get; set; }
        public int State { get; set; }
        public int PUID { get; set; }
        public string Comments { get; set; }

        public override string ToString()
        {
            return FullName;
        }

        #region Static

        public static ELiteBedLogItem Empty => new ELiteBedLogItem()
        {
            ID = 0,
            BUID = 42,
            ResNumber = "42",
            UID = 42,
            FullName = "六娃",
            ReservedDate = new DateTime(2015, 6, 13),
            IsPaid = false,
            Deposit = 0,
            Price = 0,
            Discount = 100,
            State = 42,
            PUID = 42,
            Comments = string.Empty
        };

        #endregion

        #region IELiteItem

        public bool DeleteFrom(ELiteConnection conn)
        {
            return conn.Delete("log_bed", "id", ID);
        }

        public bool InsertTo(ELiteConnection conn)
        {
            return conn.Insert("log_bed", this.ToDictionary());
        }

        public bool IsExistedIn(ELiteConnection conn)
        {
            return conn.IsExisted("log_bed", "id", ID);
        }

        public void LoadFromDataRow(DataRow row)
        {
            this.ID = (int)row["id"];
            this.BUID = (int)row["BUID"];
            this.ResNumber = (string)row["ResNumber"];
            this.UID = (int)row["uid"];
            this.ReservedDate = (DateTime)row["ReservedDate"];
            this.IsPaid = (bool)row["IsPaid"];
            this.Deposit = (float)row["Deposit"];
            this.Price = (float)row["Price"];
            this.Discount = (int)row["Discount"];
            this.State = (int)row["State"];
            this.PUID = (int)row["PUID"];
            this.Comments = (string)row["Comments"];
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "BUID", BUID },
                { "ResNumber", ResNumber },
                { "uid", UID },
                { "ReservedDate", ReservedDate },
                { "IsPaid", IsPaid },
                { "Deposit", Deposit },
                { "Price", Price },
                { "Discount", Discount },
                { "State", State },
                { "PUID", PUID },
                { "Comments", Comments }
            };
        }

        public bool UpdateIn(ELiteConnection conn)
        {
            return conn.Update("log_bed", this.ToDictionary(), "id", ID);
        }

        #endregion

    }

    #endregion

    #region ELitePickUpLogItem

    public struct ELitePickUpLogItem : IELiteTableItem
    {
        public int PUID { get; set; }
        public string FlightNumber { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string DepartureCity { get; set; }
        public int Airport { get; set; }
        public string Terminal { get; set; }
        public int CTID { get; set; }

        public override string ToString()
        {
            return FlightNumber;
        }

        #region Static

        public static ELitePickUpLogItem Empty => new ELitePickUpLogItem()
        {
            PUID = 42,
            FlightNumber = "YI6674",
            ArrivalDateTime = new DateTime(2015, 6, 13),
            DepartureCity = "小樽",
            Airport = 42,
            Terminal = "",
            CTID = 42
        };

        #endregion

        #region IELiteItem

        public bool DeleteFrom(ELiteConnection conn)
        {
            return conn.Delete("log_pickup", "PUID", PUID);
        }

        public bool InsertTo(ELiteConnection conn)
        {
            return conn.Insert("log_pickup", this.ToDictionary());
        }

        public bool IsExistedIn(ELiteConnection conn)
        {
            return conn.IsExisted("log_pickup", "PUID", PUID);
        }

        public void LoadFromDataRow(DataRow row)
        {
            this.PUID = (int)row["PUID"];
            this.FlightNumber = (string)row["ResNumber"];
            this.ArrivalDateTime = (DateTime)row["ReservedDate"];
            this.DepartureCity = (string)row["DepartureCity"];
            this.Airport = (int)row["Airport"];
            this.Terminal = (string)row["Terminal"];
            this.CTID = (int)row["CTID"];
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "PUID", PUID },
                { "ResNumber", FlightNumber },
                { "ReservedDate", ArrivalDateTime },
                { "DepartureCity", DepartureCity },
                { "Airport", Airport },
                { "Terminal", Terminal },
                { "CTID", CTID }
            };
        }

        public bool UpdateIn(ELiteConnection conn)
        {
            return conn.Update("log_pickup", this.ToDictionary(), "PUID", PUID);
        }

        #endregion

    }

    #endregion
    
}
