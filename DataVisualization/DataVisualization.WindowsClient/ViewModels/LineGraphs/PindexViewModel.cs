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
using System.Diagnostics;

namespace DataVisualization.WindowsClient.ViewModels.LineGraphs
{
    class PindexViewModel : ViewModelBase
    {
        public ObservableCollection<PindexModel> Data { get; private set; }

        public ObservableCollection<PindexModel> Average { get; private set; }

        public PindexViewModel()
        {
            RefreshCommand.Execute(null);
        }
        public void RefreshChart()
        {
            MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["dataBeest"].ConnectionString);
            conn.Open();

            // Create SQL commands
            MySqlCommand command1 = conn.CreateCommand();
            command1.CommandText = "SELECT AVG(tt.pindex), wc.temperaturegc FROM weather_condition AS wc, twitter_tweets AS tt WHERE wc.date = tt.weather_date GROUP BY wc.temperaturegc;";

            MySqlCommand command2 = conn.CreateCommand();
            command2.CommandText = "SELECT AVG(pindex) FROM twitter_tweets";

            // Create lists and vars
            Data    = new ObservableCollection<PindexModel>();
            Average = new ObservableCollection<PindexModel>();
            List<double> Temperatures = new List<double>();

            // Execute command1
            using (MySqlDataReader reader = command1.ExecuteReader())
            {
                while (reader.Read())
                {
                    Data.Add(new PindexModel { Temperature = reader.GetDouble(1), Pindex = (double)reader.GetDecimal(0) });
                    Temperatures.Add(reader.GetDouble(1));
                }
            }
            conn.Close();

            // Execute command2
            conn.Open();

            double result = double.Parse(command2.ExecuteScalar().ToString());

            for (int i = 0; i < Temperatures.Count; i++)
            {
                Average.Add(new PindexModel { Temperature = Temperatures.ElementAt(i), Pindex = result });
            }

            OnPropertyChanged(nameof(Data));
            OnPropertyChanged(nameof(Average));

            // Close database after execution
            conn.Close();
        }

        public ICommand RefreshCommand => new DelegateCommand((x) => new Task(RefreshChart).Start());
    }
}

