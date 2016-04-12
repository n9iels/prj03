using System.Configuration;
using DataVisualization.Windows;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;

namespace DataVisualization.WindowsClient.ViewModels {
    public class GraphViewModel : ViewModelBase {
        public ObservableCollection<PieChart> Data { get; private set; }

        public GraphViewModel()
        {
            MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["dataBeest"].ConnectionString);
            conn.Open();

            // Create SQL command
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "SELECT language, COUNT(id) AS amount FROM twitter_tweets GROUP BY language HAVING COUNT(id) > 75 AND language <> 'NULL' AND language <> 'UN_NotReferenced'";

            Data = new ObservableCollection<PieChart>();

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string language = reader.GetValue(0).ToString();
                    int amount      = reader.GetInt32(1);

                    Data.Add(new PieChart() { Category = language, Number = amount });
                }
            }

            // Close database after execution
            conn.Close();

            
        }

        private object selectedItem = null;
        public object SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                // selected item has changed
                selectedItem = value;
            }
        }
    }

    // class which represent a data point in the chart
    public class PieChart
    {
        public string Category { get; set; }

        public int Number { get; set; }
    }
}
