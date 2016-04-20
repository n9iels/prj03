using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Buienradar_Server {
    public class XmlFilter
    {

        /// <summary>
        /// Filters the specified <see cref="XDocument"/> and station code.
        /// </summary>
        public static Dictionary<string, string> FilterData(XDocument doc, int stationCode)
        {
            XElement filteredElements = doc.Descendants("weerstation").Where(d => (int)d.Attribute("id") == stationCode).Select(d => d).Single();

            return CreateList(filteredElements);
        }

        private static Dictionary<string, string> CreateList(XElement element)
        {
            Dictionary<string, string> data = new Dictionary<string, string> {
                { "datum", element.Element("datum").Value },
                { "temperatuurGC", element.Element("temperatuurGC").Value },
                { "windsnelheidMS", element.Element("windsnelheidMS").Value },
                { "windsnelheidBF", element.Element("windsnelheidBF").Value },
                { "luchtvochtigheid", element.Element("luchtvochtigheid").Value },
                { "regenMMPU", element.Element("regenMMPU").Value },
                { "id", element.Element("icoonactueel").Attribute("ID").Value },
                { "type", element.Element("icoonactueel").Attribute("zin").Value }
            };


            return data;
        }
    }
}
