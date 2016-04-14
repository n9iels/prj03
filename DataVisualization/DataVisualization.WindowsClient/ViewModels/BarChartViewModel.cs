using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataVisualization.Data.Models.BarChartModel;
using DataVisualization.Windows;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using System.Windows.Data;

namespace DataVisualization.WindowsClient.ViewModels
{
    class BarChartViewModel : ViewModelBase
    {
        public ObservableCollection<BarChartModel> bier { get; private set; }
        private object _lockObject = new object();

        public BarChartViewModel()
        {
            bier = new ObservableCollection<BarChartModel>();
            BindingOperations.EnableCollectionSynchronization(bier, _lockObject);
            new Task(UpdateChart).Start();

        }

        public void UpdateChart()
        {
            List<double> alles = new List<double>();
            int hallo = 0;
            string connectionString = "SERVER=daniel-molenaar.com;" + "DATABASE=danielmo_project3;" +"UID=danielmo_pr3Clnt;" + "PASSWORD=-*AGq=3wA_aJV=V;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            MySqlCommand command1 = conn.CreateCommand();
            MySqlCommand command2 = conn.CreateCommand();
            command1.CommandText = "SELECT COUNT(tt.id), wt.name FROM twitter_tweets AS tt, weather_condition AS wc, weather_types AS wt WHERE tt.pindex != 0 AND wt.id = wc.id AND wc.date = (SELECT date FROM weather_condition WHERE date < tt.created_at ORDER BY date DESC LIMIT 1) GROUP BY wt.name";
            command2.CommandText = "SELECT COUNT(tt.id), wt.name FROM twitter_tweets AS tt, weather_condition AS wc, weather_types AS wt WHERE tt.pindex != 0 AND tt.pindex > 0 AND wt.id = wc.id AND wc.date = (SELECT date FROM weather_condition WHERE date < tt.created_at ORDER BY date DESC LIMIT 1) GROUP BY wt.name";
            MySqlDataReader Reader = command1.ExecuteReader();
            while (Reader.Read())
            {
                alles.Add((Reader.GetDouble(0)/100));  
            }
            conn.Close();
            conn.Open();
            Reader = command2.ExecuteReader();
            while (Reader.Read())
            {
                double perc = Reader.GetDouble(0) / alles[hallo];
                hallo += 1;
                var hond = new BarChartModel { Count = perc,Positief = Reader.GetString(1) };
                bier.Add(hond);
            }
            conn.Close();
            OnPropertyChanged(nameof(bier));
        }
    }
}