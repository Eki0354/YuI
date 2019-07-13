using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using IOExtension;
using Microsoft.Win32;

namespace Ran
{
    public partial class SettingWindow : Window
    {
        public static string AppLang;
        public static SettingWindow swform;
        public SettingWindow()
        {
            InitializeComponent();
            /*swform = this;
            string langString = "en-us,English(the United State)|zh-cn,中文(简体)|zh-hk,中文(繁體)|ja-jp,日本語|ko-kr,한국의|de-de,Duits";
            foreach (string lang in langString.Split(Convert.ToChar("|")))
            {
                string[] langItems = lang.Split(Convert.ToChar(","));
                LanguageItem l = new LanguageItem(langItems[0], langItems[1]);
                swform.comboBox_lang.Items.Add(l);
                if (l.ID == AppLang) { swform.comboBox_lang.SelectedItem = l; }
            }*/
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
            /*LanguageItem l = comboBox_lang.SelectedItem as LanguageItem;
            if(l.ID!=AppLang) { ChangeLanguage(l.ID); }
            if (checkBox_startup.IsChecked == true) { }
            if (checkBox_sc_dt.IsChecked == true) { }
            if (checkBox_sc_sm.IsChecked == true) { }
            if (checkBox_sc_ql.IsChecked == true) { }
            Close();*/
            if (comboBox_Background.SelectedIndex < 0) return;
            ChangeBackground((comboBox_Background.SelectedItem as BackgroundImage).Title);
            this.DialogResult = true;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(BackgroundImage bg in GetBGs())
            {
                comboBox_Background.Items.Add(bg);
            }
        }
        
        public static XDocument RanXDoc { get; set; } =
            XDocument.Load(MementoPath.RanConfigPath);

        public static IEnumerable<BackgroundImage> GetBGs()
        {
            return from e in RanXDoc.Descendants("BG")
                   select new BackgroundImage
                   {
                       Name = (string)e.Attribute("name"),
                       Title = (string)e.Attribute("title")
                   };
        }

        public static bool IsExistBG(string name) =>
            (from e in RanXDoc.Descendants("BG")
             where (string)e.Attribute("name") == name
             select e).Count() > 0;

        public static string GetSelectedBGName() => GetBGName(GetSelectedBGTitle());

        public static string GetBGName(string title) =>
            (from e in GetBGs()
             where (string)e.Title == title
             select e).First().Name;

        public static string GetSelectedBGTitle() => (string)GetBG().Attribute("value");

        public static XElement GetBG() =>
            (from e in RanXDoc.Descendants("Path")
             where (string)e.Attribute("tag") == "Background"
             select e).First();

        public void ChangeBackground(string title)
        {
            GetBG().Attribute("value").Value = title;
            RanXDoc.Save(MementoPath.RanConfigPath);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog()
            {
                Filter = "图片|*.png;*.jpg;*.bmp",
                Multiselect = false,
                InitialDirectory = MementoPath.RanImagesPath
            };
            if (fd.ShowDialog().GetValueOrDefault())
            {
                if (IsExistBG(fd.SafeFileName))
                {
                    foreach(var item in comboBox_Background.Items)
                    {
                        if(item is BackgroundImage cbg)
                        {
                            if (cbg.Name == fd.SafeFileName)
                                comboBox_Background.SelectedItem = item;
                        }
                    }
                }
                else
                {
                    if (Path.GetDirectoryName(fd.FileName) != MementoPath.RanImagesPath)
                        File.Copy(fd.FileName, MementoPath.RanImagesPath + @"\" + fd.SafeFileName);
                    XElement bg = new XElement("BG");
                    bg.Add(new XAttribute("name", fd.SafeFileName));
                    bg.Add(new XAttribute("title", fd.SafeFileName.Substring(
                        0, fd.SafeFileName.LastIndexOf("."))));
                    RanXDoc.Element("Ran").Add(bg);
                    RanXDoc.Save(MementoPath.RanConfigPath);
                    BackgroundImage bgi = new BackgroundImage()
                    {
                        Name = bg.Attribute("name").Value,
                        Title = bg.Attribute("title").Value
                    };
                    comboBox_Background.Items.Add(bgi);
                    comboBox_Background.SelectedItem = bgi;
                }
            }
        }

        private void ComboBox_Background_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(sender is ComboBox cb)
            {
                if(cb.SelectedItem is BackgroundImage bg)
                {
                    image_Background.Source = new BitmapImage(
                        new Uri(string.Format("pack://siteoforigin:,,,/Images/{0}", bg.Name)));
                }
            }
        }
    }

    public class BackgroundImage
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public override string ToString()
        {
            return Title;
        }
    }
}
