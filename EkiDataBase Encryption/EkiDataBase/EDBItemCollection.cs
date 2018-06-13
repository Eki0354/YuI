using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using EkiDataBase.ItemSet;
using System.Collections;

namespace EkiDataBase
{
    public class EDBItemCollection : IEnumerable
    {
        protected List<EDBItem> Items = new List<EDBItem>();
        protected string _tableName = "";
        protected string _mainKey = "";
        //初始化EDBItemCollection类型
        public EDBItemCollection(string tableName, string mainKey)
        {
            _tableName = tableName;
            _mainKey = mainKey;
        }
        //增加一项EDBItem类型
        public void Add(EDBItem item)
        {
            if (Items.Find(x => x.Key() == item.Key()) == null) { Items.Add(item); }
        }
        //根据指定的Key值返回对应的Value
        public object GetValue(string key)
        {
            return Items.Find(x => x.Key() == key).GetValue();
        }
        //根据指定的Key值，将对应的Value更改为指定的值
        public void SetValue(string key, object value)
        {
            Items.Find(x => x.Key() == key).SetValue(value);
        }
        //从DataRow加载数据，需要先初始化Items
        public void Load(DataRow r)
        {
            Items.ForEach(x => x.SetValue(r[x.Key()]));
        }
        //返回列表中所有有效项的Key值
        public List<string> Keys()
        {
            List<string> keys = new List<string>();
            Items.FindAll(x => x.Valid()).ForEach(x => keys.Add(x.Key()));
            return keys;
        }
        //返回列表中所有有效项的Value值
        public List<object> Values()
        {
            List<object> values = new List<object>();
            Items.FindAll(x => x.Valid()).ForEach(x => values.Add(x.GetValue()));
            return values;
        }
        /// <summary>
        /// 获取列表中所有有效项的key和value值，并分别返回List列表
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        public void Output(out List<string> keys, out List<object> values)
        {
            List<string> keyC = new List<string>();
            List<object> valueC = new List<object>();
            Items.FindAll(x => x.Valid()).ForEach(x => { keyC.Add(x.Key()); valueC.Add(x.GetValue()); });
            keys = keyC;
            values = valueC;
        }
        /// <summary>
        /// 获取列表中所有项的key和value值，并分别返回List列表
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        public void FullOutput(out List<string> keys, out List<object> values)
        {
            List<string> keyC = new List<string>();
            List<object> valueC = new List<object>();
            Items.ForEach(x => { keyC.Add(x.Key()); valueC.Add(x.GetValue()); });
            keys = keyC;
            values = valueC;
        }
        /// <summary>
        /// 获取列表中所有有效项的key和value值的string类型，并分别返回List列表；如需返回原始格式，请使用Output方法。
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        public void OutputToString(out List<string> keys, out List<string> values)
        {
            List<string> keyC = new List<string>();
            List<string> valueC = new List<string>();
            Items.FindAll(x => x.Valid()).ForEach(x => { keyC.Add(x.Key()); valueC.Add(x.GetValue().ToString()); });
            keys = keyC;
            values = valueC;
        }
        /// <summary>
        /// 获取列表中所有项的key和value值的string类型，并分别返回List列表；如需返回原始格式，请使用Output方法。
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        public void FullOutputToString(out List<string> keys, out List<string> values)
        {
            List<string> keyC = new List<string>();
            List<string> valueC = new List<string>();
            Items.ForEach(x => { keyC.Add(x.Key()); valueC.Add(x.GetValue().ToString()); });
            keys = keyC;
            values = valueC;
        }
        //获取列表中所有项的Key值
        public List<string> FullKeys()
        {
            List<string> keys = new List<string>();
            Items.ForEach(x => keys.Add(x.Key()));
            return keys;
        }
        //获取列表中所有项的Value值
        public List<object> FullValues()
        {
            List<object> values = new List<object>();
            Items.ForEach(x => values.Add(x.GetValue()));
            return values;
        }
        //获取列表中所有有效项的key和value值，并返回为字典类型
        public Dictionary<string, object> Pairs()
        {
            Dictionary<string, object> pairs = new Dictionary<string, object>();
            Items.ForEach(x => { if (x.Valid()) { pairs.Add(x.Key(), x.GetValue()); } } );
            return pairs;
        }
        //获取列表中所有项的key和value值，并返回为字典类型
        public Dictionary<string, object> FullPairs()
        {
            Dictionary<string, object> pairs = new Dictionary<string, object>();
            Items.ForEach(x => pairs.Add(x.Key(), x.GetValue()));
            return pairs;
        }
        public string InsertString()
        {
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            OutputToString(out keys, out values);
            return "Insert into " + _tableName + " (" + String.Join(",", keys.ToArray()) 
                + ") Values (" + String.Join(",", values.ToArray()) + ")";
        }
        public string UpdateString()
        {
            string sql = "Update " + _tableName + " set ";
            foreach(KeyValuePair<string,object > p in Pairs())
            {
                sql += p.Key + "=" + p.Value.ToString() + ",";
            }
            return sql.Substring(0, sql.Length - 1) + " where " + _mainKey + "=" + GetValue(_mainKey).ToString();
        }
        public string DeleteString()
        {
            return "Delete from " + _tableName + " where " + _mainKey + "=" + GetValue(_mainKey).ToString();
        }
        public IEnumerator GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
