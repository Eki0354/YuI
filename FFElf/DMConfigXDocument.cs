using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IOExtension;
using dmNet;

namespace FFElf
{
    public class DMConfigXDocument
    {
        static XDocument _DMDoc;
        static string _Size;

        static DMConfigXDocument()
        {
            _DMDoc = XDocument.Load(MementoPath.DMConfigPath);
            dmsoft dm = new dmsoft();
            _Size = string.Format("{0}x{1}",
                dm.GetScreenWidth(),
                dm.GetScreenHeight());
            dm = null;
        }

        public static string GetImageName(string tag) =>
            (from e in _DMDoc.Descendants("Image")
             where e.Attribute("tag").Value == tag && e.Attribute("size").Value == _Size
             select e).First().Attribute("name").Value;

        public static (int, int) GetCoord(string tag) =>
            (from e in _DMDoc.Descendants("Coord")
             where e.Attribute("tag").Value == tag && e.Attribute("size").Value == _Size
             select (Convert.ToInt32(e.Attribute("x").Value),
             Convert.ToInt32(e.Attribute("y").Value))).First();
    }
}
