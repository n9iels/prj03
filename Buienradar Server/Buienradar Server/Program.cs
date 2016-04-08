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
            string LastDate = "";

            while (true)
            {
                Dictionary<string, string> element = GetWeatherStation(6344);

                if (LastDate != element["datum"])
                {
                    // Do something wit the database
                    InsertIntoDatabase(element);

                    // Set last date
                    LastDate = element["datum"];
                }

                System.Threading.Thread.Sleep(300000);
            }
        }

        public static Dictionary<string, string> GetWeatherStation (int id)
        {
            XDocument doc = XDocument.Load("http://xml.buienradar.nl");
            XElement filteredElements = doc.Descendants("weerstation").Where(d => (int)d.Attribute("id") == id).Select(d => d).Single();

            return CreateList(filteredElements);
        }

        public static Dictionary<string, string> CreateList(XElement element)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("stationnaam", element.Element("stationnaam").Value);
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

        
        public static void InsertIntoDatabase(Dictionary<string, string> data)
        {
            // Connect to the database
            string connectionString = ConfigurationManager.ConnectionStrings["dataBeest"].ConnectionString;
            conn = new MySqlConnection(connectionString);
            conn.Open();

            // Create SQL command
            MySqlCommand command = conn.CreateCommand();
            command.CommandText  = "INSERT IGNORE INTO weather_condition VALUES(@date, @temperaturegc, @windspeedms, @windspeedbf, @humidity, @rainmmpu);";
            command.CommandText += "INSERT IGNORE INTO weather_types VALUES(@id, @name);";
            command.Parameters.AddWithValue("@date", DateTime.Parse(data["datum"]));
            command.Parameters.AddWithValue("@temperaturegc", data["temperatuurGC"]);
            command.Parameters.AddWithValue("@windspeedms", data["windsnelheidMS"]);
            command.Parameters.AddWithValue("@windspeedbf", data["windsnelheidBF"]);
            command.Parameters.AddWithValue("@humidity", data["luchtvochtigheid"]);
            command.Parameters.AddWithValue("@rainmmpu", data["regenMMPU"]);
            command.Parameters.AddWithValue("@id", data["id"]);
            command.Parameters.AddWithValue("@name", data["stationnaam"]);
            command.ExecuteNonQuery();

            conn.Close();
        }
        
    }
}
