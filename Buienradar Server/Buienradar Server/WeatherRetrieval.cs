using System.Xml.Linq;

namespace Buienradar_Server {
    public class WeatherRetrieval {

        /// <summary>
        /// Gets the Xml document from the specified url.
        /// </summary>
        /// <param name="url">Url with the location of the xml document.</param>
        public static XDocument GetData(string url) {
            XDocument doc = XDocument.Load(url);

            return doc;
        }
    }
}