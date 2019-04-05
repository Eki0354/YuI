using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Ran
{
    public class SavedPasswordElf
    {
        public static string PasswordFilePath = Environment.CurrentDirectory + @"\sp.db";
        
        public static Dictionary<string,string> ReadPasswords()
        {
            Dictionary<string, string> passwords = new Dictionary<string, string>();
            FileStream fs = new FileStream(PasswordFilePath, FileMode.OpenOrCreate);
            StreamReader sr = new StreamReader(fs);
            while (!sr.EndOfStream)
            {
                string text = sr.ReadLine();
                int index = text.IndexOf(";;;");
                passwords.Add(text.Substring(0, index), text.Substring(index + 3));
            }
            sr.Close();
            fs.Close();
            return passwords;
        }

        public static void SavePassword(string name,string password)
        {
            Dictionary<string, string> passwords = ReadPasswords();
            if (passwords.ContainsKey(name))
                passwords[name] = password;
            else
                passwords.Add(name, password);
            FileStream fs = new FileStream(PasswordFilePath, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            foreach(KeyValuePair<string, string> pair in passwords)
            {
                sw.WriteLine(pair.Key + ";;;" + pair.Value);
            }
            sw.Close();
            fs.Close();
        }
    }
}
