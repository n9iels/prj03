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
    class DayViewModel : ViewModelBase
    {
        public ObservableCollection<DayModel> Data { get; private set; }

        public DayViewModel()
        {
            RefreshCommand.Execute(null);
        }

        public void RefreshChart()
        {
            MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["dataBeest"].ConnectionString);
            conn.Open();

            // Create SQL commands
            MySqlCommand command1 = conn.CreateCommand();
            command1.CommandText = "SELECT tt.created_at, AVG(tt.pindex) FROM twitter_tweets AS tt WHERE created_at >= '2016-4-8 00:00:00' GROUP BY DATE(tt.created_at)";

            // Create lists and vars
            Data = new ObservableCollection<DayModel>();

            // Execute command1
            using (MySqlDataReader reader = command1.ExecuteReader())
            {
                while (reader.Read())
                {
                    Data.Add(new DayModel { Date = reader.GetDateTime(0), Pindex = (double)reader.GetDecimal(1) });
                }
            }
            conn.Close();

            OnPropertyChanged(nameof(Data));
        }

        public ICommand RefreshCommand => new DelegateCommand((x) => new Task(RefreshChart).Start());
    }
}
