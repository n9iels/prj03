using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Xml.Linq;

namespace Buienradar_Server {
    public class Program {
        private static string _last;
        private static void Main()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            while (true)
            {
                // Get whole document
                try {

                    XDocument doc = WeatherRetrieval.GetData("http://xml.buienradar.nl");
                    Dictionary<string, string> data = XmlFilter.FilterData(doc, 6344);

                    if (_last == null || data["datum"] != _last) {
                        DatabaseParser.UploadToDatabase(data);
                        _last = data["datum"];

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
