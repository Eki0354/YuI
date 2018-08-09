using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ELite;
using System.Windows.Controls;
using EkiXmlDocument;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using ControlItemCollection;

namespace Reservation
{
    public enum ResGettingMethod { CLIPBOARD, HTTP }

    public static class Getter
    {
        private static EXmlReader _XmlReader;
        public static string[] DateTimeFormatArray;

        /// <summary> 初始化订单获取器，需要路径参数，默认为类库工作路径，此路径用于
        /// 初始化获取器的xml配置文件路径 </summary>
        public static void InitializeGetter(string path = "")
        {
            if (path == null || path == "" || !Directory.Exists(path))
                path = Environment.CurrentDirectory;
            _XmlReader = new EXmlReader(path, "res");
            DateTimeFormatArray = _XmlReader.ReadValue("SourceSetting/DateTimeFormatItems")
                .Split(Convert.ToChar("|"));
        }
        
        /// <summary> 对传入的文本信息进行分析，提取订单号及订单详情，成功则返回订单号，
        /// 失败时返回空值。 </summary>
        public static ListBoxResItem GetReservation(ELiteConnection conn, string text)
        {
            ListBoxResItem res = GetResNumber(text);
            if (res.ResNumber.Length == 0 || res.Channel.Length == 0) return null;
            //若标记为HTTP获取，需要自动登录获取订单页面文本，将替换被提取订单信息的文本
            ResGettingMethod method = (ResGettingMethod)(
                Convert.ToInt32(_XmlReader.ReadValue(res.Channel + "/Method")));
            if (method == ResGettingMethod.HTTP) text = GetHttpHtmlText(res);
            //从订单源文本中分析提取订单详情，成功返回订单，失败返回空值
            if (GetRes(res, text)) return res;
            return null;
        }

        private static ListBoxResItem GetResNumber(string text)
        {
            Regex regex;
            ListBoxResItem item = new ListBoxResItem(string.Empty, string.Empty);
            //列出所有订单源，并尝试获取订单号
            foreach (XmlNode node in _XmlReader.ReadNodes("Channels"))
            {
                //检测剪贴板文本中是否含有当前订单源的关键词，不含有则跳过
                if (text.IndexOf(node.SelectSingleNode("Keyword").InnerText) < 0) continue;
                //使用正则表达式匹配订单号
                regex = new Regex(node.SelectSingleNode("Rex-ResNumber").InnerText);
                item = new ListBoxResItem(regex.Match(text).Value, node.Name);
                if (item.ResNumber.Length != 0) break;
            }
            return item;
        }

        /// <summary> 为Method为Http获取方式的订单源获取页面代码 </summary>
        private static string GetHttpHtmlText(ListBoxResItem res)
        {

            return string.Empty;
        }

        /// <summary> 使用正则表达式从订单源文本中匹配订单详情，并保存到数据库。 </summary>
        private static bool GetRes(ListBoxResItem res, string text)
        {
            Dictionary<string, string> resDict = RegexResItems(
                _XmlReader.ReadNode("Channel/" + res.Channel + "/Rex-Res"), text);
            Dictionary<string, string> userDict = RegexResItems(
                _XmlReader.ReadNode("Channel/" + res.Channel + "/Rex-User"), text);
            List<Dictionary<string, string>> roomDictList = RegexRoomItems(res.Channel, text);
            if (resDict.Count < 1 || userDict.Count < 1 || roomDictList == null) return false;
            //转换DateTime格式字符串为标准格式

            return true;
        }

        /// <summary> 使用正则表达式从源文本中获取房间详情字典的列表。 </summary>
        private static List<Dictionary<string, string>> RegexRoomItems(string channel, string text)
        {
            string roomRegexText = _XmlReader.ReadValue(
                "Channel/" + channel + "/Rex-Room");//正则表达式文本
            if (string.IsNullOrEmpty(roomRegexText)) return null;
            Regex regex = new Regex(roomRegexText);
            MatchCollection matches = regex.Matches(text);//预订的房间通常不止一个
            if (matches.Count < 1) return null;
            List<Dictionary<string, string>> roomDictList = new List<Dictionary<string, string>>();
            foreach (Match roomMatch in matches)
            {
                roomDictList.Add(RegexResItems(
                    _XmlReader.ReadNode("Channel/" + channel + "/Rex-User"), roomMatch.Value));
            }

            
            return roomDictList;
        }
        
        /// <summary> 通用方法。使用正则表达式从源文本中获取详情字典。 </summary>
        private static Dictionary<string,string> RegexResItems(XmlNode rexNode, string text)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (XmlNode cNode in rexNode.ChildNodes)
            {
                string nodeName = cNode.Name;
                string regexText = cNode.InnerText;
                if (string.IsNullOrEmpty(regexText)) continue;
                Regex regex = new Regex(regexText);
                string result = regex.Match(text).Value;
                if (string.IsNullOrEmpty(result)) continue;
                dict.Add(cNode.Name, result);
            }
            return dict;
        }

        private static void SaveRes(ELiteConnection conn, Dictionary<string, object> resDict,
            List<Dictionary<string, object>> roomDict)
        {
            conn.InsertRes(resDict, roomDict);
        }

        private static Dictionary<string,string> GetDateTimeReplaceItems(string channel)
        {
            Dictionary<string, string> items = new Dictionary<string, string>();
            foreach (string repText in _XmlReader.ReadValue(
                "Sources/" + channel + "DateTimeReplaceItems").Split(Convert.ToChar("|")))
            {
                if (repText.Length < 3) continue;
                int index = repText.IndexOf(",");
                if (index < 0) continue;
                if (index == 1 && repText.Substring(2, 1) == ",") index = 2;
                //以上代码确保英文逗号可替换和可被替换
                items.Add(repText.Substring(0, index), repText.Substring(index + 1));
            }
            return items;
        }

        private static void InitializeDateTimeItems(Dictionary<string,object > items)
        {

        }
    }

    
}
