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
            IEnumerable<XElement> elements = getWeatherStation(6344);
            
            foreach(XElement element in elements)
            {
                Console.WriteLine(element);
                Console.WriteLine("1");
            }
        }

        public static IEnumerable<XElement> getWeatherStation (int id)
        {
            XDocument doc = XDocument.Load("http://xml.buienradar.nl");
            IEnumerable<XElement> filteredElements = (from d
                                                      in doc.Descendants("weerstation")
                                                      where (int)d.Attribute("id") == id
                                                      select d).ToList();

            return filteredElements;
        }

        public static void database()
        {
            // Connect to the database
            string connectionString = ConfigurationManager.ConnectionStrings["dataBeest"].ConnectionString;
            conn = new MySqlConnection(connectionString);
            conn.Open();

            // Create SQL command
            MySqlCommand command = conn.CreateCommand();
            MySqlDataReader Reader;
            command.CommandText = "SELECT * FROM weather_condition";

            // Read result
            Reader = command.ExecuteReader();
            while (Reader.Read())
            {
                Console.WriteLine(Reader.GetValue(1).ToString());
            }
            conn.Close();
        }
    }
}
