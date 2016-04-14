using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Threading;
using System.Globalization;

namespace Buienradar_Server
{
    class Program {
        private static string last = null;
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            while (true)
            {
                // Get whole document
                try {

                    XDocument doc = WeatherRetrieval.GetData("http://xml.buienradar.nl");
                    Dictionary<string, string> data = XmlFilter.FilterData(doc, 6344);

                    if (last == null || data["datum"] != last) {
                        DatabaseParser.UploadToDatabase(data);
                        last = data["datum"];

                        // Console log
                        Console.WriteLine("Weather updated " + data["datum"]);
                    }
  
                }
                catch (WebException)
                {
                    Console.WriteLine("Unable to load XML Data");
                }

                // Wait 1 minute
                Thread.Sleep(1 * 1000 * 60);
            }
        }
    }
}
