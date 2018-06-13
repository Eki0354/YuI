using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkiDataBase.ItemSet
{
    public class EDBStringItem : EDBItem
    {
        string _value = "";
        public EDBStringItem(string key) { _key = key; }
        public override void SetValue(object obj)
        {
            try
            {
                _value = obj.ToString();
                _isSet = true;
            }
            catch
            {

            }
        }
        public override object GetValue()
        {
            return _value;
        }
        public override string ToString()
        {
            return "'" + _value + "'";
        }
    }
}
