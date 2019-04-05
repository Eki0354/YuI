using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingElf
{
    public enum BubbleBookingState
    {
        Normal,
        Changed,
        Cancelled,
        Invalid
    }

    #region BubbleBookingItem

    /// <summary> 仅用于在BubbleListBox中绑定显示的数据类型 </summary>
    public class BubbleBookingItem
    {
        public string Channel { get; }
        public string FullName { get; }
        public string ResNumber { get; }
        public string DisplayName => 
            FullName.Length > 20 ? FullName.Substring(0, 17) + "..." : FullName;
        public BubbleBookingState State { get; set; } = BubbleBookingState.Normal;
        public bool IsSearchResult { get; set; } = false;

        public BubbleBookingItem(string channel, string fullName, string resNumber)
        {
            this.Channel = channel;
            this.FullName = fullName;
            this.ResNumber = resNumber;
        }

        public static BubbleBookingItem FromDataRow(DataRow row, bool isSearchResult = false)
        {
            try
            {
                BubbleBookingItem item = new BubbleBookingItem(
                    row["Channel"] as string,
                    row["FullName"] as string,
                    row["ResNumber"] as string)
                {
                    IsSearchResult = isSearchResult,
                    State = (BubbleBookingState)Enum.Parse(typeof(BubbleBookingState), row["State"].ToString())
                };
                return item;
            }
            catch
            {
                Console.WriteLine(row);
                throw new Exception("非标准数据，无法生成BubbleBookingItem实例！");
            }
        }

        public bool AxEquals(BubbleBookingItem item) =>
            this.Channel == item.Channel &&
            this.FullName == item.FullName &&
            this.ResNumber == item.ResNumber;

        public bool CompareToResNumber(string resNumber) => this.ResNumber == resNumber;

        public bool CompareToFullName(string fullName) =>
            this.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase);

    }

    #endregion

    #region BubbleBookingItemCollection
    
    public class BubbleBookingItemCollection: ObservableCollection<BubbleBookingItem>
    {
        public BubbleBookingItemCollection(IEnumerable<BubbleBookingItem> collection) : base(collection)
        {

        }

        public BubbleBookingItemCollection()
        {

        }

        #region Add

        public void AddToFirst(BubbleBookingItem item)
        {
            this.Insert(0, item);
        }

        public void Add(IEnumerable<BubbleBookingItem> items)
        {
            foreach(BubbleBookingItem item in items)
            {
                this.AddToFirst(item);
            }
        }

        public void Add(DataTable table)
        {
            this.Add(FromDataTable(table));
        }

        #endregion

        #region Contains

        public bool AxContains(BubbleBookingItem cItem)
        {
            foreach(BubbleBookingItem item in this.Items)
            {
                if (cItem.AxEquals(item)) return true;
            }
            return false;
        }

        public bool ContainsOfResNumber(string resNumber)
        {
            foreach (BubbleBookingItem item in this.Items)
            {
                if (item.CompareToResNumber(resNumber)) return true;
            }
            return false;
        }

        public bool ContainsOfFullName(string fullName)
        {
            foreach (BubbleBookingItem item in this.Items)
            {
                if (item.CompareToFullName(fullName)) return true;
            }
            return false;
        }

        #endregion

        #region IndexOf

        public int AxIndexOf(BubbleBookingItem item)
        {
            for(int i = 0; i < this.Count; i++)
            {
                if (this[i].AxEquals(item)) return i;
            }
            return -1;
        }

        public int IndexOfResNumber(string resNumber)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].CompareToResNumber(resNumber)) return i;
            }
            return -1;
        }

        public int IndexOfFullName(string fullName)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].CompareToFullName(fullName)) return i;
            }
            return -1;
        }

        #endregion

        #region First

        public BubbleBookingItem AxFirst(BubbleBookingItem item)
        {
            try
            {
                return this.First(rItem => rItem.AxEquals(item));
            }
            catch(InvalidOperationException ex)
            {
                return null;
            }
        }

        public BubbleBookingItem FirstOfResNumber(string resNumber)
        {
            try
            {
                return this.First(item => item.CompareToResNumber(resNumber));
            }
            catch (InvalidOperationException ex)
            {
                return null;
            }
        }

        public BubbleBookingItem FirstOfFullName(string fullName)
        {
            try
            {
                return this.First(item => item.CompareToResNumber(fullName));
            }
            catch (InvalidOperationException ex)
            {
                return null;
            }
        }

        #endregion

        public static BubbleBookingItemCollection FromDataTable(DataTable table, bool isSearchResult = false)
        {
            BubbleBookingItemCollection items = new BubbleBookingItemCollection();
            foreach (DataRow row in table.Rows)
            {
                items.Add(BubbleBookingItem.FromDataRow(row, isSearchResult));
            }
            return items;
        }
    }

    #endregion

    #region ConceivingBookingItem

    /// <summary> 仅用于获取订单时使用的临时数据类型 </summary>
    public class ConceivingBookingItem
    {
        public long UID { get; set; }
        public long ID { get; set; }
        public string Channel { get; set; }
        public string FullName { get; set; }
        public string ResNumber { get; set; }
        public BubbleBookingState State { get; set; } = BubbleBookingState.Normal;

        public BubbleBookingItem ToBubble()
        {
            return new BubbleBookingItem(Channel, FullName, ResNumber)
            {
                State = this.State
            };
        }
    }

    #endregion

}
