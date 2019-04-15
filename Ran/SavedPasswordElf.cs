using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IOExtension;

namespace Ran
{
    public class SavedPasswordElf
    {
        public static Dictionary<string, string> ReadPasswords()
        {
            Dictionary<string, string> passwords = new Dictionary<string, string>();
            using (FileStream fs = new FileStream(MementoPath.SavedPasswordFilePath, FileMode.OpenOrCreate))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                    {
                        string text = sr.ReadLine();
                        int index = text.IndexOf(";;;");
                        if (index < 0) continue;
                        passwords.Add(text.Substring(0, index), text.Substring(index + 3));
                    }
                }
            }
            return passwords;
        }

        public static void SavePassword(string name,string password)
        {
            if (MementoPath.SavedPasswordFilePath.Contains("OneDrive") &&
                Environment.UserName != "Mrs Panda") return;
            Dictionary<string, string> passwords = ReadPasswords();
            if (passwords.ContainsKey(name))
                passwords[name] = password;
            else
                passwords.Add(name, password);
            using (FileStream fs = new FileStream(MementoPath.SavedPasswordFilePath, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach (KeyValuePair<string, string> pair in passwords)
                    {
                        sw.WriteLine(pair.Key + ";;;" + pair.Value);
                    }
                }
            }
        }
    }
}
