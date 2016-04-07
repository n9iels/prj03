using System;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace Buienradar_Server
{
    class Program
    {
        public static MySqlConnection conn;

        static void Main(string[] args)
        {
            // Connect to the database
            string connectionString = ConfigurationManager.ConnectionStrings["dataBeest"].ConnectionString;
            conn = new MySqlConnection(connectionString);
            conn.Open();

            // Get weather data
            Dictionary<string, string> elements = getWeatherStation(6344);

            // Create SQL command
            MySqlCommand command = conn.CreateCommand();
            MySqlDataReader Reader;
            command.CommandText = "SELECT * FROM weather_condition";

            conn.Close();
        }

        public static Dictionary<string, string> getWeatherStation (int id)
        {
            XDocument doc = XDocument.Load("http://xml.buienradar.nl");
            IEnumerable<XElement> filteredElements = doc.Descendants("weerstation").Where(d => (int)d.Attribute("id") == id).Select(d => d).ToList();

            return createList(filteredElements);
        }

        public static Dictionary<string, string> createList(IEnumerable<XElement> elements)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
        
            foreach (XElement element in elements)
            {
                data.Add("datum", element.Element("datum").Value);
                data.Add("temperatuurGC", element.Element("temperatuurGC").Value);
                data.Add("windsnelheidBF", element.Element("windsnelheidBF").Value);
                data.Add("id", element.Element("icoonactueel").Attribute("ID").Value);
                data.Add("type", element.Element("icoonactueel").Attribute("zin").Value);
            }

            return data;
        }
    }
}
