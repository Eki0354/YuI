using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ran
{
    public class APTXItem
    {
        public int SID { get; }
        public string Nickname { get; }
        public int Sex { get; }
        public DateTime Birth { get; }
        public int Identity { get; }
        internal string SavedPassword { get; set; }
        string Password { get; }
        string Salt { get; }

        public APTXItem(int sid, string nickname, int sex, DateTime birth, int identity, 
            string password, string salt)
        {
            this.SID = sid;
            this.Nickname = nickname;
            this.Sex = sex;
            this.Birth = birth;
            this.Identity = identity;
            this.Password = password;
            this.Salt = salt;
        }

        public override string ToString()
        {
            return Nickname;
        }

        public bool EqualsPassword(string code) => GetEncryptedPassword(code + Salt) == Password;
        
        public static string GetEncryptedPassword(string currentText)
        {
            SHA256 hash = SHA256.Create();
            byte[] s = hash.ComputeHash(Encoding.Unicode.GetBytes(currentText));
            return Encoding.Unicode.GetString(s);
        }

        public static APTXItem FromDataRow(DataRow row)
        {
            try
            {
                APTXItem item = new APTXItem(
                    (int)row["sid"],
                    row["Nickname"] as string,
                    (int)row["Sex"],
                    (DateTime)row["Birth"],
                    (int)row["Identity"],
                    row["Password"] as string,
                    row["Salt"] as string);
                return item;
            }
            catch
            {
                Console.WriteLine(row);
                throw new Exception("非标准数据，无法生成APTXItem实例！");
            }
        }

    }

    public static class ListExtension
    {
        public static List<T> GetDisruptedItems<T>(this List<T> list)
        {
            //生成一个新数组：用于在之上计算和返回
            List<T> temp = new List<T>();
            list.ForEach(item => temp.Add(item));
            //打乱数组中元素顺序
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < temp.Count; i++)
            {
                int x, y; T t;
                x = rand.Next(0, temp.Count);
                do
                {
                    y = rand.Next(0, temp.Count);
                } while (y == x);
                t = temp[x];
                temp[x] = temp[y];
                temp[y] = t;
            }
            return temp;
        }

        public static List<APTXItem> GetDisruptedAPTX(this List<APTXItem> items)
        {
            if (!items.Contains("Eki")) items.Add(
                new APTXItem(307, "Eki", 1, new DateTime(1993, 03, 07), 0, "", ""));
            if (!items.Contains("Mori")) return items;
            items = items.GetDisruptedItems();
            int eI = items.FindIndex("Eki");
            int mI = items.FindIndex("Mori");
            if (Math.Abs(eI - mI) != 1)
            {
                if (eI < mI)
                {
                    items.Exchange(eI, mI - 1);
                }
                else
                {
                    items.Exchange(mI, eI - 1);
                }
            }
            return items;
        }

        public static bool Contains(this List<APTXItem> items, string nickname) =>
            items.FindIndex(nickname) > -1;

        public static int FindIndex(this List<APTXItem> items, string nickname) =>
            items.FindIndex(item => item.Nickname == nickname);

        public static void Exchange(this List<APTXItem> items, int aI, int bI)
        {
            APTXItem itemA = items[aI];
            items[aI] = items[bI];
            items[bI] = itemA;
        }
    }

}
