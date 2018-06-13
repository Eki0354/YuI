using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using EkiDataBase;
using EkiDataBase.ItemSet;

namespace Reservation
{
    public enum RoomType : byte { MIX_6, MIX_4, FEMALE_4, MIX_3, MIX_2, SINGLE_BUDGET, SINGLE_BASIC, TWIN_BASIC, DOUBLE_BASIC,
        TRIPLE_BASIC, SINGLE_STANDARD, TWIN_STANDARD, DOUBLE_STANDARD, TRIPLE_STANDARD, FAMILY }
    public class RoomItemCollection : EDBItemCollection
    {
        public RoomItemCollection(DataRow r = null) : base("info_res_room", "Order")
        {
            Add(new EDBStringItem("Order"));
            Add(new EDBStringItem("Guest"));
            Add(new EDBDateTimeItem("Date"));
            Add(new EDBByteItem("Type"));//参考文档：房间\房型代码对应表
            Add(new EDBByteItem("Rooms"));
            Add(new EDBByteItem("Persons"));
            Add(new EDBSingleItem("Price"));
            Add(new EDBBooleanItem("Cancelled"));
            Add(new EDBDateTimeItem("Recorded"));
            //用于读取订单时，传入非空数据行，按列标题赋值
            if (r != null)
            {
                Items.ForEach(x => x.SetValue(r[x.Key()]));
            }
            
        }
    }
}
