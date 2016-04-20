using System;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace TwitterApi.Data_Processors {
    internal static class MySqlQueryGenerator {

        /// <summary>
        /// Generates a MySql command using the specified <see cref="MySqlConnection"/>, query and objects.
        /// </summary>
        internal static MySqlCommand GenerateQuery(MySqlConnection connection, string query, params object[] parameters) {
            MatchCollection paramMatches = GetParameters(query);
            if (paramMatches.Count != parameters.Length)
                throw new ArgumentException(
                    "The amount of parameters did not match the amount of objects passed to the method");
            MySqlCommand command = new MySqlCommand() {
                CommandText = query,
                Connection = connection
            };

            for (int i = 0; i < paramMatches.Count; i++)
                command.Parameters.AddWithValue(paramMatches[i].Value, parameters[i]);

            return command;
        }

        /// <summary>
        /// Generates a MySql command using the specified query and objects.
        /// </summary>

        internal static MySqlCommand GenerateQuery(string query, params object[] parameters) {
            return GenerateQuery(null, query, parameters);
        }

        private static MatchCollection GetParameters(string query) {
            Regex reg = new Regex(@"(?<!@)@\w+");
            return reg.Matches(query);
        }
    }
}
