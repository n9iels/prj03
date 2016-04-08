using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Buienradar_Server
{
    class DatabaseParser
    {
        public static void UploadToDatabase(Dictionary<string, string> data)
        {
            // Connect to the database
            string connectionString = ConfigurationManager.ConnectionStrings["dataBeest"].ConnectionString;
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();

            // Create SQL command
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "INSERT IGNORE INTO weather_condition VALUES(@date, @temperaturegc, @windspeedms, @windspeedbf, @humidity, @rainmmpu);";
            command.CommandText += "INSERT IGNORE INTO weather_types VALUES(@id, @name);";
            command.Parameters.AddWithValue("@date", DateTime.Parse(data["datum"]));
            command.Parameters.AddWithValue("@temperaturegc", data["temperatuurGC"]);
            command.Parameters.AddWithValue("@windspeedms", data["windsnelheidMS"]);
            command.Parameters.AddWithValue("@windspeedbf", data["windsnelheidBF"]);
            command.Parameters.AddWithValue("@humidity", data["luchtvochtigheid"]);
            command.Parameters.AddWithValue("@rainmmpu", data["regenMMPU"]);
            command.Parameters.AddWithValue("@id", data["id"]);
            command.Parameters.AddWithValue("@name", data["type"]);
            command.ExecuteNonQuery();
               
            // Close database after execution
            conn.Close();
        }
    }
}
