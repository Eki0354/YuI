using System;
using System.Globalization;
using System.IO;
using System.Windows;
using IOExtension;

namespace Ran
{
    public partial class SettingWindow : Window
    {
        public static string AppLang;
        public static SettingWindow swform;
        public SettingWindow()
        {
            InitializeComponent();
            swform = this;
            string langString = "en-us,English(the United State)|zh-cn,中文(简体)|zh-hk,中文(繁體)|ja-jp,日本語|ko-kr,한국의|de-de,Duits";
            foreach (string lang in langString.Split(Convert.ToChar("|")))
            {
                string[] langItems = lang.Split(Convert.ToChar(","));
                LanguageItem l = new LanguageItem(langItems[0], langItems[1]);
                swform.comboBox_lang.Items.Add(l);
                if (l.ID == AppLang) { swform.comboBox_lang.SelectedItem = l; }
            }
        }

        public static void LoadSettings()
        {
            //if ((!File.Exists(Environment.CurrentDirectory + @"\accounts.db"))) { CreateNewDataBase(); }
            if ((!File.Exists(Environment.CurrentDirectory + @"\settings.ini")))
            {
                StreamWriter sw = new StreamWriter(
                new FileStream(Environment.CurrentDirectory + @"\settings.ini", FileMode.Create));
                sw.Write(Properties.Resources.settings);
                sw.Close();
            }
            if (AppLang == null) { AppLang = CultureInfo.InstalledUICulture.Name.ToLower(); }
            ChangeLanguage(AppLang);
            if (!File.Exists(MementoPath.EmailTemplatesDirectory + @"\订单确认.txt"))
                File.Copy(Environment.CurrentDirectory + @"\订单确认.txt",
                    MementoPath.EmailTemplatesDirectory + @"\订单确认.txt");
            if (!File.Exists(MementoPath.EmailTemplatesDirectory + @"\订单确认_Html.txt"))
                File.Copy(Environment.CurrentDirectory + @"\订单确认_Html.txt",
                    MementoPath.EmailTemplatesDirectory + @"\订单确认_Html.txt");
        }

        private static void CreateNewDataBase()
        {

        }

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button_confirm_Click(object sender, RoutedEventArgs e)
        {
            LanguageItem l = comboBox_lang.SelectedItem as LanguageItem;
            if(l.ID!=AppLang) { ChangeLanguage(l.ID); }
            if (checkBox_startup.IsChecked == true) { }
            if (checkBox_sc_dt.IsChecked == true) { }
            if (checkBox_sc_sm.IsChecked == true) { }
            if (checkBox_sc_ql.IsChecked == true) { }
            Close();
        }

        private static void ChangeLanguage(string lang)
        {
            if (AppLang != lang)
            {
                ResourceDictionary langRd = null;
                try
                {
                    langRd = Application.LoadComponent(
                    new Uri("Lang/" + lang + ".xaml", UriKind.Relative)) as ResourceDictionary;
                }
                catch
                {
                }
                if (langRd != null)
                {
                    ResourceDictionary rd = Application.Current.Resources;
                    if (rd.MergedDictionaries.Count > 0)
                    {
                        rd.MergedDictionaries.Clear();
                    }
                    rd.MergedDictionaries.Add(langRd);
                    AppLang = lang;
                }
            }
        }
        private class LanguageItem
        {
            public LanguageItem(string id, string displayName)
            {
                this.ID = id;
                this.DisplayName = displayName;
            }
            public string ID { get; set; }
            public string DisplayName { get; set; }

            public override string ToString()
            {
                return DisplayName;
            }
        }
    }
}
