using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ELite
{
    public class Logger
    {
        FileStream _FS;
        StreamWriter _SW;
        private string _Path;
        private string _Name;
        private string _Extension;
        public string FullName => _Name + "." + _Extension;
        public string FullPath => _Path + "\\" + FullName;

        public Logger(string fullPath = "")
        {
            InitializePath(fullPath);
        }
        
        private void InitializePath(string fullPath = "")
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
                _Name = "HROS";
                _Extension = "log";
            }
        }

        public Logger(string path,string name="HROS",string extension = "log")
        {
            InitializePath(path, name, extension);
        }

        private void InitializePath(string path, string name, string extension)
        {
            if (path == "") path = Environment.CurrentDirectory;
            if (name == "") name = "HROS";
            if (extension == "") extension = "log";
            _Path = path;
            _Name = name;
            _Extension = extension;
        }

        public void Open()
        {
            if (!File.Exists(FullPath))
                File.Create(FullPath);
            _FS = new FileStream(FullPath, FileMode.Append, FileAccess.Write);
            _SW = new StreamWriter(_FS);
        }

        public void Refresh(string fullPath = "")
        {
            Close();
            InitializePath(fullPath);
            Open();
        }

        public void Refresh(string path, string name = "HROS", string extension = "log")
        {
            Close();
            InitializePath(path, name, extension);
            Open();
        }

        public void Close()
        {
            if (_SW != null)
                _SW.Close();
            _SW = null;

            if (_FS != null)
                _FS.Close();
            _FS.Close();
        }

        public void Log(string title, string errorText)
        {
            string log = "Time: " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + 
                "\r\nTitle: " + title + "\r\nMessage: " + errorText + "\r\n";
            _SW.WriteLine(log);
        }

        public void Log(string title, Exception e)
        {
            string log = "Time: " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + 
                "\r\nTitle: " + title + "\r\nMessage: " + e.Message + "\r\n";
            _SW.WriteLine(log);
        }
    }
}
