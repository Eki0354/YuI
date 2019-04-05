using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using CCBItem = Interface_Reception_Ribbon.EControls.ChannelComboBoxItem;
using CCBIList = Interface_Reception_Ribbon.EControls.ChannelComboBoxItemList;

namespace Interface_Reception_Ribbon.EControls
{

    public class BubbleResCollection : ObservableCollection<BubbleResListBoxItem>
    {
        public BubbleResCollection(IEnumerable<BubbleResListBoxItem> collection) : base(collection)
        {

        }

        public BubbleResCollection()
        {

        }
        
        public BubbleResListBoxItem FirstByResNumber(string resNumber)
        {
            try
            {
                return this.First(bubble => bubble.ResNumber == resNumber);
            }
            catch
            {
                return null;
            }
        }

        public BubbleResListBoxItem FirstByFullName(string fullName)
        {
            try
            {
                return this.First(bubble => bubble.FullName == fullName);
            }
            catch
            {
                return null;
            }
        }

        public BubbleResListBoxItem FirstByChannel(string channel)
        {
            try
            {
                return this.First(bubble => bubble.Channel == channel);
            }
            catch
            {
                return null;
            }
        }

        public void AddToFirst(BubbleResListBoxItem item)
        {
            this.Insert(0, item);
        }

        public CCBIList Channels
        {
            get
            {
                CCBIList channels = new CCBIList()
                {
                    CCBItem.All
                };
                foreach (BubbleResListBoxItem bubble in this)
                {
                    if (!channels.Contains(bubble.Channel))
                        channels.Add(new CCBItem(bubble.Channel));
                }
                return channels;
            }
        }

        public static BubbleResCollection FromDataTable(DataTable table, bool isSearchResult = false)
        {
            BubbleResCollection items = new BubbleResCollection();
            foreach (DataRow row in table.Rows)
            {
                items.Add(BubbleResListBoxItem.FromDataRow(row, isSearchResult));
            }
            return items;
        }
    }

    public enum BubbleResState
    {
        Normal,
        Cancelled,
        Invalid
    }

    public class BubbleResListBoxItem
    {
        public string Channel { get; }
        public string FullName { get; }
        public string ResNumber { get; }
        public BubbleResState State { get; set; } = BubbleResState.Normal;
        public bool IsSearchResult { get; set; }

        public BubbleResListBoxItem(string channel, string fullName, string resNumber, bool isSearchResult = false)
        {
            this.Channel = channel;
            this.FullName = fullName;
            this.ResNumber = resNumber;
            this.IsSearchResult = isSearchResult;
        }

        public static BubbleResListBoxItem FromDataRow(DataRow row, bool isSearchResult = false)
        {
            try
            {
                BubbleResListBoxItem item = new BubbleResListBoxItem(
                    row["Channel"] as string,
                    row["FullName"] as string,
                    row["ResNumber"] as string,
                    isSearchResult)
                {
                    State = (BubbleResState)Enum.Parse(typeof(BubbleResState), row["State"].ToString())
                };
                return item;
            }
            catch
            {
                Console.WriteLine(row);
                throw new Exception("非标准数据，无法生成BubbleResItem实例！");
            }
        }
    }

    public class AliveBubbleResListBoxItem
    {
        public long UID { get; set; }
        public long ID { get; set; }
        public string Channel { get; set; }
        public string FullName { get; set; }
        public string ResNumber { get; set; }
        public BubbleResState State { get; set; } = BubbleResState.Normal;
        public bool IsSearchResult { get; set; }

        public BubbleResListBoxItem ToBubble()
        {
            return new BubbleResListBoxItem(Channel, FullName, ResNumber, IsSearchResult)
            {
                State = this.State
            };
        }
    }
}
