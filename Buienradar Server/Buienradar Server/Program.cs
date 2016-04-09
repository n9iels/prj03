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
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            while (true)
            {
                // Get whole document
                try {

                    XDocument doc = WeatherRetrieval.GetData("http://xml.buienradar.nl");
                    Dictionary<string, string> data = XmlFilter.FilterData(doc, 6344);
                    DatabaseParser.UploadToDatabase(data);

                    // Console log
                    Console.WriteLine("Weather updated " + data["datum"]);
                }
                catch (WebException)
                {
                    Console.WriteLine("Unable to load XML Data");
                }

                // Wait 5 minutes
                System.Threading.Thread.Sleep(5 * 1000 * 60);
            }
        }
    }
}
