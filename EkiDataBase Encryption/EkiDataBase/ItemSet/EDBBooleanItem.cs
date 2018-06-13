using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkiDataBase.ItemSet
{
    public class EDBBooleanItem : EDBItem
    {
        bool _value = false;
        public EDBBooleanItem(string key) { _key = key; }
        public override void SetValue(object obj)
        {
            try
            {
                _value = Convert.ToBoolean(obj);
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
            return _value ? "1" : "0";
        }
    }
}
