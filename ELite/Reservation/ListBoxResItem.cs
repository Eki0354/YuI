using ELite.ELiteItem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELite.Reservation
{
    /// <summary> 用于在ListBoxResBalloon控件中的订单属性类。 </summary>
    public class ELiteListBoxResItem:IELiteTableItem
    {
        public long ID { get; private set; }
        public string Channel { get; set; }
        public string ResNumber { get; set; }
        public string FullName { get; set; }
        public bool IsValid { get; private set; }
        public bool IsSearchResult { get; set; } = false;

        public ELiteListBoxResItem()
        {

        }

        public override string ToString()
        {
            return Channel + (!IsValid ? " 无效" : "") + (IsSearchResult ? " 查" : "") + "\r\n" + FullName;
        }

        public override bool Equals(object obj)
        {
            ELiteListBoxResItem res = obj as ELiteListBoxResItem;
            if (res == null) return false;
            return ID == res.ID &&
                Channel == res.Channel &&
                ResNumber == res.ResNumber &&
                FullName == res.FullName &&
                IsValid == res.IsValid &&
                IsSearchResult == res.IsSearchResult;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region Static

        public static ELiteListBoxResItem Empty => new ELiteListBoxResItem
        {
            Channel = "Mars",
            ResNumber = "42",
            FullName = "六娃",
            IsValid = true,
            IsSearchResult = false
        };
        
        #endregion

        #region IELiteItem

        /// <summary> 此方法不受支持，仅为实现接口。返回错误提示。</summary>
        public bool InsertTo(ELiteConnection conn)
        {
            throw new Exception("不允许此类向数据库直接发送插入命令！");
        }

        /// <summary> 此方法不受支持，仅为实现接口。返回错误提示。</summary>
        public bool DeleteFrom(ELiteConnection conn)
        {
            throw new Exception("不允许此类向数据库直接发送删除命令！");
        }

        /// <summary> 此方法不受支持，仅为实现接口。返回错误提示。 </summary>
        public bool UpdateIn(ELiteConnection conn)
        {
            throw new Exception("不允许此类向数据库直接发送更新命令！");
        }

        public bool IsExistedIn(ELiteConnection conn)
        {
            return conn.IsExisted("info_res", "ResNumber", ResNumber);
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "id", ID },
                { "Channel", Channel },
                { "ResNumber", ResNumber },
                { "FullName", FullName },
                { "IsValid", IsValid }
            };
        }

        public void LoadFromDataRow(DataRow row)
        {
            this.Channel = (string)row["Channel"];
            this.ResNumber = (string)row["ResNumber"];
            this.FullName = (string)row["FullName"];
            this.IsValid = (bool)row["IsValid"];
            this.ID = (long)row["id"];
        }

        public void Invalid(ELiteConnection conn)
        {
            conn.Run("update info_res set IsValid=0 where ResNumber='" + this.ResNumber + "'",
                Operation.UPDATE, "info_res");
            //Run("delete from info_res_rooms where ResNumber='" + resNumber + "'",
            //Operation.DELETE, "info_res_rooms");
        }

        public void InitializeID(ELiteConnection conn)
        {
            this.ID = conn.GetItem<long>("select max(id) from info_res where ResNumber='" +
                ResNumber + "' and IsValid=1");
        }

        #endregion

    }
}
