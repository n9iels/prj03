using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Buienradar_Server
{
    class XmlFilter
    {
        public static Dictionary<string, string> FilterData(XDocument doc, int stationCode)
        {
            XElement filteredElements = doc.Descendants("weerstation").Where(d => (int)d.Attribute("id") == stationCode).Select(d => d).Single();

            return CreateList(filteredElements);
        }

        private static Dictionary<string, string> CreateList(XElement element)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("datum", element.Element("datum").Value);
            data.Add("temperatuurGC", element.Element("temperatuurGC").Value);
            data.Add("windsnelheidMS", element.Element("windsnelheidMS").Value);
            data.Add("windsnelheidBF", element.Element("windsnelheidBF").Value);
            data.Add("luchtvochtigheid", element.Element("luchtvochtigheid").Value);
            data.Add("regenMMPU", element.Element("regenMMPU").Value);
            data.Add("id", element.Element("icoonactueel").Attribute("ID").Value);
            data.Add("type", element.Element("icoonactueel").Attribute("zin").Value);

            return data;
        }
    }
}
