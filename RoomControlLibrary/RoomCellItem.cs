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
                _items.ForEach(x => x.SetValue(r[x.Key()]));
            }
        }
        public RoomCellItemCollection(int number, DateTime date) : base("info_room", "Number")
        {
            Initialize();

        }
        void Initialize()
        {
            _items.Add(new EDBIntegerItem("Number"));
            _items.Add(new EDBByteItem("Type"));
            _items.Add(new EDBStringItem("Order"));
            _items.Add(new EDBStringItem("UID"));
            _items.Add(new EDBStringItem("Channel"));
            _items.Add(new EDBStringItem("Name"));
        }
    }
}
