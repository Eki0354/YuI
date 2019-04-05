using System;
using System.Collections.Generic;
using System.Xml;

namespace MementoConnection
{
    /// <summary> 此类被设计为ResConfig.Xml文件的读写Helper类，继承自XMLDocument类。除Channels节点外，其余Configuration根节点
    /// 下的子节点皆称为表节点（FogNode），其对应的子节点称为里节点（OtherNode）。 </summary>
    public class EXmlDocument : XmlDocument
    {
        XmlNode _RootNode;

        public override void Load(string filename)
        {
            base.Load(filename);
            _RootNode = this.FirstChild;
            if (_RootNode.Name != "Configuration")
                throw new Exception("Xml文件根节点错误！");
        }

        #region Base

        protected XmlNode ReadOtherNode(string fogName, string otherName) =>
            _RootNode.SelectSingleNode(fogName).SelectSingleNode(otherName);

        protected string ReadOtherNodeText(string fogName, string otherName) => ReadOtherNode(fogName, otherName).InnerText;
        
        protected XmlNodeList ReadOtherNodes(string fogName) =>
            _RootNode.SelectSingleNode(fogName).ChildNodes;

        protected Dictionary<string, string> ReadOtherNodePairs(string fogName)
        {
            Dictionary<string, string> nodePairs = new Dictionary<string, string>();
            foreach(XmlNode node in ReadOtherNodes(fogName))
            {
                nodePairs.Add(node.Name, node.InnerText);
            }
            return nodePairs;
        }

        #endregion
        
    }
    
}
