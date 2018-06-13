using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace Interface_Reception_Ribbon
{
    public class res_js
    {
        public void AddNewOrder(string[] order)
        {
            //将传递进来的字符串数组转为字典列表
            List<Dictionary<string, string>> items = new List<Dictionary<string, string>>();
            string[] orderKeys = order[0].Split(Convert.ToChar(","));
            string[] roomKeys = order[1].Split(Convert.ToChar(","));
            string[] keys = orderKeys;
            string[] values;
            for (int i = 2; i < order.Length; i++)
            {
                Dictionary<string, string> item = new Dictionary<string, string>();
                values = order[i].Split(Convert.ToChar(","));
                for (int j = 0; j < values.Length; j++)
                {
                    item.Add(keys[j], values[j]);
                }
                items.Add(item);
                keys = roomKeys;
            }
            //保存所有条目；
            string form_order = "res_order";
            string form_room = "res_room";
            string form = form_order;
            string sql = "";
            items.ForEach(item =>
            {
                sql = "insert " + form + " (" +
                    String.Join(",", item.Keys.ToArray()) + ") values(" +
                    String.Join(",", item.Values.ToArray()) + ")";
                //如果保存出错，则返回
                form = form_room;
            });

        }
        private List<Dictionary<string, string>> AdornedItems(string[] order)
        {
            List<Dictionary<string, string>> items = new List<Dictionary<string, string>>();
            string[] orderKeys = order[0].Split(Convert.ToChar(","));
            string[] roomKeys = order[1].Split(Convert.ToChar(","));
            string[] keys = orderKeys;
            string[] values;
            for(int i = 2; i < order.Length; i++)
            {
                Dictionary<string, string> item = new Dictionary<string, string>();
                values = order[i].Split(Convert.ToChar(","));
                for(int j = 0; j < values.Length; j++)
                {
                    item.Add(keys[j], values[j]);
                }
                items.Add(item);
                keys = roomKeys;
            }
            return items;
        }
        public void NewOrder(string site, string account, string password, string id)
        {

        }
    }
}
