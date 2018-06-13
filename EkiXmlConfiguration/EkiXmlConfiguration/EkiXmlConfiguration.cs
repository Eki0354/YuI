using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace EkiXmlConfiguration
{
    public class XmlConfiguration
    {
        private XmlDocument doc;
        public XmlNode Node { get; private set; }
        public string InnerText { get; private set; }
        public string Source { get; set; }
        public string errorText { get; private set; }
        public string FilePath { get; private set; }
        private string NodePath;
        /// <summary>
        /// If the file does not exist, it will be created with a main node named "Configure".
        /// </summary>
        /// <param name="path"></param>
        public XmlConfiguration(string path = "")
        {
            FilePath = path;
            if (FilePath == "") { FilePath = Environment.CurrentDirectory + "\\resconfig.xml"; }
            if (!File.Exists(FilePath)){ Create(); }
            doc = new XmlDocument();
            try
            {
                doc.Load(FilePath);
            }
            catch (Exception ex)
            {
                errorText = "Failed to load the file!" + ex.Message;
            }
        }
        private void Create()
        {
            FileStream fs= File.Open(FilePath, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Configure>\r\n</Configure>");
            sw.Close();
            fs.Close();
        }
        private void CreateSourceNode()
        {
            doc.SelectSingleNode("Configure").SelectSingleNode("Sources").AppendChild(doc.CreateElement(Source));
        }
        /// <summary>
        /// Automaticlly return the childnodes of "Sources".
        /// </summary>
        /// <returns></returns>
        public XmlNodeList SourceNodes()
        {
            return doc.SelectSingleNode("Configure").SelectSingleNode("Sources").ChildNodes;
        }
        /// <summary>
        /// return the child node of "Source" which name is the string inputed.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public XmlNode SelectSourceNode()
        {
            return doc.SelectSingleNode("Configure").SelectSingleNode("Sources").SelectSingleNode(Source);
        }
        public XmlNodeList ResNodes()
        {
            return SelectSourceNode().SelectSingleNode("Rex-Res").ChildNodes;
        }
        public XmlNodeList RoomNodes()
        {
            return SelectSourceNode().SelectSingleNode("Rex-Room").ChildNodes;
        }
        /// <summary>
        /// return the node which path is inputed string. use "," to divide.
        /// </summary>
        /// <param name="nodepath"></param>
        /// <returns></returns>
        public XmlNode Select(string nodepath)
        {
            string[] paths = nodepath.Split(Convert.ToChar(","));
            XmlNode node = doc.SelectSingleNode("Configure");
            foreach (string path in paths)
            {
                node = node.SelectSingleNode(path);
            }
            return node;
        }
        /// <summary>
        /// return the innertext of the node which path is inputed string. use "," to divide.
        /// </summary>
        /// <param name="nodepath"></param>
        /// <returns></returns>
        public bool Read(string nodepath)
        {
            try
            {
                string[] paths = nodepath.Split(Convert.ToChar(","));
                XmlNode node = doc.SelectSingleNode("Configure");
                foreach (string path in paths)
                {
                    node = node.SelectSingleNode(path);
                }
                this.Node = node;
                NodePath = nodepath;
                InnerText = node.InnerText;
                return true;
            }
            catch (Exception ex)
            {
                errorText = "Failed to read the node!\r\n" + "The path of:Configure/" + nodepath.Replace(",", "/") + "\r\n" + ex.Message;
                return false;
            }
        }
        public bool ReadSourceNode(string nodename)
        {
            try
            {
                InnerText = SelectSourceNode().SelectSingleNode(nodename).InnerText;
                return true;
            }
            catch (Exception ex)
            {
                errorText = "Failed to read the node!\r\n" + "The path of:Configure/" + Source + "/" + nodename + "\r\n" + ex.Message;
                return false;
            }
        }
        /// <summary>
        /// change the innertext of the node. If input empty path, change the current node. If else, use "," to divide path.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="nodepath"></param>
        /// <returns></returns>
        public bool Change(string value, string nodepath = "")
        {
            try
            {
                if (nodepath != "") { Read(nodepath); }
                this.Node.InnerText = value;
                return true;
            }
            catch (Exception ex)
            {
                errorText = "Failed to change the node!\r\n" + "The path of:Configure/" + NodePath.Replace(",", "/") + "\r\n" + ex.Message;
                return false;
            }
        }
        public bool Save()
        {
            try
            {
                doc.Save(FilePath);
                return true;
            }
            catch (Exception ex)
            {
                errorText = "Failed to save file!\r\n" + ex.Message;
                return false;
            }
        }
        public bool CreateNewResXml()
        {
            //创建来源共有订单节点项
            List<XmlElement> resxel = new List<XmlElement>();
            foreach (string str in ("SourceLanguage,ShopName,CustomerName,AreaFrom,EmailAddress,Deposit,Address,PhoneNumber," +
                "Sex,PreferredLanguage,BookedDate,ArrivalDate,DepartureDate,TotalCost,Status,Persons,ArrivalTime,Nights," +
                "Rooms,Balance,Channel,Commission,ConfirmCode,ContactPerson,ContactPhoneNumber,LastUpdateDate,RecordedDate,SmokingRoom"
                 + "FoodMeal").Split(Convert.ToChar(",")))
            {
                resxel.Add(doc.CreateElement(str));
            }
            //创建网络来源共有订单节点项
            List<XmlElement> netxel = new List<XmlElement>();
            foreach(string str in ("NeedConfirm,Post-Login,Site-Login,Site-Res,Site-Default,Site-NewResList,Site-Confirm,Post-Confirm,Rex-NewRev").Split(Convert.ToChar(",")))
            {
                netxel.Add(doc.CreateElement(str));
            }
            //创建来源共有房间节点项
            List<XmlElement> roomxel = new List<XmlElement>();
            foreach (string str in "GuestName,ArrivalDate,DepartureDate,RoomType,Price,Rooms,Persons,Nights,Status".Split(Convert.ToChar(",")))
            {
                roomxel.Add(doc.CreateElement(str));
            }
            //创建来源共有基础节点项
            List<XmlElement> sourcexel = new List<XmlElement>();
            foreach (string str in "IsNet,Rex-Source,Rex-Number,Rex-Rooms,Rex-Res,Rex-Room".Split(Convert.ToChar(",")))
            {
                XmlElement xe = doc.CreateElement(str);
                sourcexel.Add(xe);
                if (str == "Rex-Res")
                {
                    foreach (XmlElement resxe in resxel)
                    {
                        XmlNode nxe = resxe.Clone();
                        xe.AppendChild(nxe);
                    }
                }
                if (str == "Rex-Room")
                {
                    foreach (XmlElement roomxe in roomxel)
                    {
                        XmlNode nxe = roomxe.Clone();
                        xe.AppendChild(nxe);
                    }
                }
            }
            //添加网页订单节点
            doc.SelectSingleNode("Configure").AppendChild(doc.CreateElement("Sources"));
            foreach (string source in ("Agoda=0,Booking=0,HostelWorld=1,Qunar=0,Ctrip=0,Elong=0").Split(Convert.ToChar(",")))
            {
                Source = source.Substring(0,source.IndexOf("="));
                CreateSourceNode();
                XmlNode sxn = SelectSourceNode();
                foreach(XmlElement xe in sourcexel)
                {
                    XmlNode nxn = xe.Clone();
                    sxn.AppendChild(nxn);
                }
                if (source.Substring(source.IndexOf("=") + 1, 1) == "1")
                {
                    sxn.AppendChild(doc.CreateElement("Net"));
                    XmlNode nxn = sxn.SelectSingleNode("Net");
                    foreach (XmlElement xe in netxel)
                    {
                        XmlNode nxe = xe.Clone();
                        nxn.AppendChild(nxe);
                    }
                }
            }
            doc.Save(FilePath);
            return true;
        }
    }
}
