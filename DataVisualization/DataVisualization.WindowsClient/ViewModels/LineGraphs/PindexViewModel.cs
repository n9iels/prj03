using DataVisualization.Data.Models.LineGraphModel.LineGraphs;
using DataVisualization.Windows;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DataVisualization.WindowsClient.ViewModels.LineGraphs
{
    class PindexViewModel : ViewModelBase
    {
        public ObservableCollection<PindexModel> Data { get; private set; }

        public PindexViewModel()
        {
            RefreshCommand.Execute(null);
        }

        public void RefreshChart()
        {
            MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["dataBeest"].ConnectionString);
            conn.Open();

            // Create SQL command
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "SELECT AVG(tt.pindex), wc.temperaturegc FROM weather_condition AS wc, twitter_tweets AS tt WHERE wc.date = (SELECT date FROM weather_condition WHERE date < tt.created_at ORDER BY date DESC LIMIT 1) GROUP BY wc.temperaturegc";

            Data = new ObservableCollection<PindexModel>();

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Data.Add(new PindexModel { Temperature = reader.GetDouble(1), Pindex = reader.GetDouble(0) });
                }
            }

            OnPropertyChanged(nameof(Data));

            // Close database after execution
            conn.Close();
        }

        public ICommand RefreshCommand => new DelegateCommand((x) => new Task(RefreshChart).Start());
    }
}

