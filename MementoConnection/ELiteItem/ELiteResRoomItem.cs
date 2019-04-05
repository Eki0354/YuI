using MementoConnection.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using MMC = MementoConnection.MMConnection;

namespace MementoConnection.ELiteItem
{
    public class ELiteResRoomFieldCollection : ELiteDBItemBase
    {

        public override string TableName => Resources.TableName_ResRoom;

        public override string FieldsString => Resources.ELiteResRoomItemString;
        
        public ELiteResRoomFieldCollection(DataRow row) : base(row)
        {

        }

        public ELiteResRoomFieldCollection() : base()
        {

        }

    }

    public class ELiteResRoomItemCollection : ObservableCollection<ELiteResRoomFieldCollection>
    {
        #region Static

        public static DataTable GetEmptyVisualItemSet(DateTime _startDate, DateTime _endDate)
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("房间号", typeof(int)));
            for (DateTime date = _startDate; date < _endDate.AddDays(1); date = date.AddDays(1))
            {
                table.Columns.Add(new DataColumn(date.ToString("MM月dd日"),
                    typeof(ELiteResRoomFieldCollection)));
            }
            return table;
        }

        #endregion

        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        public DataTable VisualItemSet { get; private set; }

        public List<int> RoomIDList { get; private set; }

        public ObservableCollection<ELiteResRoomFieldCollection> UnscheduledResRoomItems { get; private set; }


        public ELiteResRoomItemCollection(DateTime startDate, DateTime endDate)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
            foreach (DataRow row in MMC.Select(string.Format(
                "select * from {0} where ReservedDate>='{1}' and ReservedDate<='{2}'",
                Resources.TableName_ResRoom,
                StartDate.ToString("yyyy-MM-dd"),
                EndDate.ToString("yyyy-MM-dd")), false).Rows)
            {
                this.Add(new ELiteResRoomFieldCollection(row));
            }
            this.RoomIDList = MMC.GetItems<int>("info_room", "RID", "Owner=0");
        }

        public void InitVisualItemSet()
        {
            DataTable dt = GetEmptyVisualItemSet(StartDate, EndDate);
            for (int i = 0; i < RoomIDList.Count; i++)
            {
                dt.Rows.Add();
                dt.Rows[i][0] = RoomIDList[i];
            }
            this.UnscheduledResRoomItems = new ObservableCollection<ELiteResRoomFieldCollection>();
            foreach (ELiteResRoomFieldCollection item in this.Items)
            {
                int cI = ((DateTime)item["ReservedDate"] - StartDate).Days + 1;
                int? rid = item["RID"] as int?;
                if (rid is null || !RoomIDList.Contains((int)rid))
                {
                    this.UnscheduledResRoomItems.Add(item);
                }
                else
                {
                    dt.Rows[RoomIDList.IndexOf((int)rid)][cI] = item;
                }
            }
            this.VisualItemSet = dt;
        }
    }
}
