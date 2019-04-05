using MementoConnection.Properties;

namespace MementoConnection.ELiteItem
{
    public class ELiteUserItem : ELiteDBItemBase
    {

        public override string TableName => Resources.TableName_User;

        public override string FieldsString => Resources.ELiteUserItemString;
        
    }
}
