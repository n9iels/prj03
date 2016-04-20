using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml.Linq;
using MySql.Data.MySqlClient;

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
                catch (WebException ex) {
                    Console.WriteLine("Unable to load XML Data");
                    try {
                        string[] lines = {
                            "==============", "Failed to load XML data from : \"http://xml.buienradar.nl\".", $"Error message : {ex.Message}" ,
                            ""
                        };
                        File.AppendAllLines("error.log", lines);
                    }
                    catch { }
                }
                catch (MySqlException ex) {
                    try {
                        string[] lines = {
                            "==============", "Failed to upload data to MySql database.",
                            $"Error message : {ex.Message}", ""
                        };
                        File.AppendAllLines("error.log", lines);
                    }
                    catch { }
                }

                // Wait 1 minute
                Thread.Sleep(1 * 1000 * 60);
            }
        }
    }
}
