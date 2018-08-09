using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using ErrorLogger;

namespace EkiXmlDocument
{
    public class EXmlReader
    {
        private XmlDocument _Doc;
        private string _Path;
        private string _Name;
        private string _Extension;
        public string FullPath { get { return _Path + "\\" + _Name + "." + _Extension; } }
        private XmlNode _Root;
        private Logger _Logger;

        #region MAIN
        
        public EXmlReader(string fullPath = "")
        {
            InitializePath(fullPath);
        }
        
        public EXmlReader(string path, string name = "config", string extension = "xml")
        {
            InitializePath(path, name, extension);
        }
        
        public void Open()
        {
            InitializeLogger();
            InitializeDoc();
        }

        private void Create()
        {
            _Doc = new XmlDocument();
            XmlDeclaration declaration = _Doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
            _Doc.AppendChild(declaration);
            _Root = _Doc.CreateElement("Configuration");
            _Doc.AppendChild(_Root);
            _Doc.Save(FullPath);
        }

        private bool Load()
        {
            _Doc = new XmlDocument();
            try
            {
                _Doc.Load(FullPath);
                _Root = _Doc.SelectSingleNode("Configuration");
                return true;
            }
            catch (Exception ex)
            {
                _Logger.Log("Failed to load the xml file!", ex.Message);
                return false;
            }
        }

        public void Refresh(string fullPath = "")
        {
            Close();
            InitializePath(fullPath);
            Open();
        }

        public void Refresh(string path, string name = "config", string extension = "log")
        {
            Close();
            InitializePath(path, name, extension);
            Open();
        }

        public void Save()
        {
            _Doc.Save(FullPath);
        }

        public void Close()
        {
            if (_Logger != null)
                _Logger.Close();
            _Logger = null;

            if (_Doc != null)
                Save();
            _Doc = null;
        }

        #endregion

        #region INITIALIZE

        private void InitializePath(string fullPath)
        {
            int index0 = fullPath.LastIndexOf("\\");
            int index1 = fullPath.LastIndexOf(".");
            if (fullPath != "" && index0 > 0 && index1 > 1 && index0 < index1)
            {
                _Path = fullPath.Substring(0, index0);
                _Name = fullPath.Substring(index0 + 1, index1 - index0 - 1);
                _Extension = fullPath.Substring(index1 + 1);
            }
            else
            {
                _Path = Environment.CurrentDirectory;
                _Name = "config";
                _Extension = "xml";
            }
        }
        
        private void InitializePath(string path, string name, string extension)
        {
            if (path == "") path = Environment.CurrentDirectory;
            if (name == "") name = "config";
            if (extension == "") extension = "xml";
            _Path = path;
            _Name = name;
            _Extension = extension;
        }

        private void InitializeLogger()
        {
            _Logger = new Logger(_Path, _Name + "." + _Extension);
            _Logger.Open();
        }

        private void InitializeDoc()
        {
            //初始化_Doc，
            if (File.Exists(FullPath))
            {
                if (!Load()) throw new Exception();
            }
            else
            {
                Create();
            }
        }

        #endregion

        #region PAIR

        public KeyValuePair<string, string> ReadPair(string keyPath)
        {
            XmlNode node = _Root.SelectSingleNode(keyPath);
            return new KeyValuePair<string, string>(node.Name, node.InnerText);
        }

        public void ReadPair(string keyPath, out string key, out string value)
        {
            XmlNode node = _Root.SelectSingleNode(keyPath);
            key = node.Name;
            value = node.InnerText;
        }

        public Dictionary<string, string> ReadPairs(string key)
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            foreach (XmlNode node in _Root.SelectSingleNode(key).ChildNodes)
            {
                keyValues.Add(node.Name, node.InnerText);
            }
            return keyValues;
        }

        #endregion
        
        #region NODE

        public XmlNode ReadNode(string keyPath)
        {
            return _Root.SelectSingleNode(keyPath);
        }

        public List<XmlNode> ReadNodes(string key)
        {
            List<XmlNode> nodes = new List<XmlNode>();
            foreach (XmlNode node in _Root.SelectSingleNode(key).ChildNodes)
            {
                nodes.Add(node);
            }
            return nodes;
        }

        #endregion

        #region KEY

        public string ReadKey(string keyPath)
        {
            return _Root.SelectSingleNode(keyPath).Name;
        }

        public List<string> ReadKeys(string keyPath)
        {
            List<string> keys = new List<string>();
            foreach(XmlNode node in _Root.SelectSingleNode(keyPath).ChildNodes)
            {
                keys.Add(node.Name);
            }
            return keys;
        }

        #endregion

        #region VALUE

        public string ReadValue(string keyPath)
        {
            return _Root.SelectSingleNode(keyPath).InnerText;
        }

        public List<string> ReadValues(string keyPath)
        {
            List<string> keys = new List<string>();
            foreach (XmlNode node in _Root.SelectSingleNode(keyPath).ChildNodes)
            {
                keys.Add(node.InnerText);
            }
            return keys;
        }

        #endregion
    }
}
