using MementoConnection.Properties;

namespace MementoConnection.ELiteItem
{
    public enum ELiteResItemState
    {
        Normal,
        Changed,
        Cancelled,
        Invalid
    }

    public class ELiteResItem : ELiteDBItemBase
    {
        public override string TableName => Resources.TableName_Res;

        public override string FieldsString => Resources.ELiteResItemString;
        
        public ELiteResRoomItemCollection RoomItems { get; }
        
    }
}
