using MementoConnection.Properties;
using System.Data;

namespace MementoConnection.ELiteItem
{
    public class ELiteRoomFieldCollection : ELiteDBItemBase
    {

        public override string TableName => Resources.TableName_Room;

        public override string FieldsString => Resources.ELiteRoomItemString;
        
        public ELiteRoomFieldCollection() : base()
        {

        }

        public ELiteRoomFieldCollection(DataRow row) : base(row)
        {

        }
        
    }



}
