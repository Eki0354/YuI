using System.Collections.ObjectModel;
using CCBItem = Interface_Reception_Ribbon.EControls.ChannelComboBoxItem;

namespace Interface_Reception_Ribbon.EControls
{
    public class ChannelComboBoxItem
    {
        public string Name { get; }
        public bool IsChecked { get; set; }

        public ChannelComboBoxItem(string name, bool isChecked = false)
        {
            this.Name = name;
            this.IsChecked = isChecked;
        }

        public static CCBItem All => new CCBItem("全部", true);

    }

    public class ChannelComboBoxItemList : ObservableCollection<CCBItem>
    {
        public ChannelComboBoxItemList() : base()
        {
            this.Add(CCBItem.All);
        }

        public bool Contains(string channelName)
        {
            foreach(CCBItem item in this)
            {
                if (item.Name == channelName)
                    return true;
            }
            return false;
        }
    }
}
