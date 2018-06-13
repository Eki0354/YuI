using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkiDataBase.ItemSet
{
    public class EDBItem
    {
        protected string _key;
        public string Key() { return _key; }
        protected bool _isSet = false;
        //public EDBItem(string key) { Key = key; }
        public virtual void SetValue(object obj) { }
        public virtual object GetValue() { return null; }
        public bool Valid() { return _isSet; }
    }
}
