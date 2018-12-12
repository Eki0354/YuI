using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELite.GridItem
{
    public class GridRoomNumberItemList : IList<GridRoomNumberItem>
    {
        private List<GridRoomNumberItem> _Items = new List<GridRoomNumberItem>();

        public GridRoomNumberItem this[int index] { get => _Items[index]; set => _Items[index] = value; }

        public int Count => _Items.Count;

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(GridRoomNumberItem item)
        {
            _Items.Add(item);
        }

        public void Clear()
        {
            _Items.Clear();
        }

        public bool Contains(GridRoomNumberItem item)
        {
            return _Items.Contains(item);
        }

        public void CopyTo(GridRoomNumberItem[] array, int arrayIndex)
        {
            _Items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<GridRoomNumberItem> GetEnumerator()
        {
            return _Items.GetEnumerator();
        }

        public int IndexOf(GridRoomNumberItem item)
        {
            return _Items.IndexOf(item);
        }

        public void Insert(int index, GridRoomNumberItem item)
        {
            _Items.Insert(index, item);
        }

        public bool Remove(GridRoomNumberItem item)
        {
            return _Items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _Items.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Items.GetEnumerator();
        }
    }

    public class GridRoomNumberItem
    {
        public int RID { get; }
        public List<int> BIDs { get; } = new List<int>();
        public int Mode { get; }

        public GridRoomNumberItem()
        {
            RID = 777;
            Mode = 0;
        }

        public GridRoomNumberItem(int rid, int mode, ELiteConnection conn)
        {
            this.RID = rid;
            this.Mode = mode;
            this.BIDs = conn.GetItems<int>("select BID from info_bed where RID=" + RID);
        }

        public List<string> RoomNumbers()
        {
            List<string> rns = new List<string>();
            switch (Mode)
            {
                case 1:
                    rns.Add(RID.ToString());
                    break;
                case 2:
                    BIDs.ForEach(bid => rns.Add(RID + "-" + bid));
                    break;
                default:
                    rns.Add("未知模式\r\n" + RID.ToString());
                    break;
            }
            return rns;
        }
    }
}
