using System;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace Buienradar_Server
{
    class WeatherRetrieval
    {
        public static XDocument GetData (string url)
        {
            XDocument doc = XDocument.Load(url);

            return doc;
        }   
    }
}
