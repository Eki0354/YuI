using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using EkiDataBase;
using EkiDataBase.ItemSet;

namespace RoomCell
{
    public class RoomCellItemCollection : EDBItemCollection
    {
        public RoomCellItemCollection(DataRow r = null) : base("info_room", "Number")
        {
            Initialize();
            if (r != null)
            {
                Items.ForEach(x => x.SetValue(r[x.Key()]));
            }
        }
        public RoomCellItemCollection(int number, DateTime date) : base("info_room", "Number")
        {
            Initialize();

        }
        void Initialize()
        {
            Items.Add(new EDBIntegerItem("Number"));
            Items.Add(new EDBByteItem("Type"));
            Items.Add(new EDBStringItem("Order"));
            Items.Add(new EDBStringItem("UID"));
            Items.Add(new EDBStringItem("Channel"));
            Items.Add(new EDBStringItem("Name"));
        }
    }
}
