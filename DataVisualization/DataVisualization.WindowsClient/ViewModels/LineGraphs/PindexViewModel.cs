using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DataVisualization.Data.Models.LineGraphModel.LineGraphs;
using DataVisualization.Windows;
using MySql.Data.MySqlClient;

namespace DataVisualization.WindowsClient.ViewModels.LineGraphs {
    public class PindexViewModel : ViewModelBase
    {
        public ObservableCollection<PindexData> Data { get; private set; }
        public ObservableCollection<PindexData> Average { get; private set; }
        private readonly PindexModel _model;

        public PindexViewModel() {
            _model = new PindexModel();

            LastAvailableDate = DateTime.Now;
            StartDate = DateTime.Now - new TimeSpan(1, 0, 0, 0);
            EndDate = LastAvailableDate;

            new Task(() => {
                using (ProjectEntities db = new ProjectEntities()) {
                    var all = from row in db.weather_condition select row.date;
                    FirstAvailableDate = all.OrderBy(x => x).Take(1).Single();
                }
            }).Start();

            RefreshCommand.Execute(null);
        }
        public void RefreshChart()
        {
            MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["dataBeest"].ConnectionString);
            conn.Open();

            // Create SQL commands
            MySqlCommand command1 = conn.CreateCommand();
            command1.CommandText = "SELECT AVG(tt.pindex), wc.temperaturegc FROM weather_condition AS wc, twitter_tweets AS tt WHERE tt.created_at >= @start AND tt.created_at <= @end AND wc.date = tt.weather_date GROUP BY wc.temperaturegc;";
            command1.Parameters.AddWithValue("@start", StartDate);
            command1.Parameters.AddWithValue("@end", EndDate);

            MySqlCommand command2 = conn.CreateCommand();
            command2.CommandText =
                "SELECT AVG(pindex) FROM twitter_tweets WHERE created_at >= @start AND created_at <= @end";
            command2.Parameters.AddWithValue("@start", StartDate);
            command2.Parameters.AddWithValue("@end", EndDate);

            // Create lists and vars
            Data    = new ObservableCollection<PindexData>();
            Average = new ObservableCollection<PindexData>();
            List<double> Temperatures = new List<double>();

            // Execute command1
            using (MySqlDataReader reader = command1.ExecuteReader())
            {
                while (reader.Read())
                {
                    Data.Add(new PindexData { Temperature = reader.GetDouble(1), Pindex = (double)reader.GetDecimal(0) });
                    Temperatures.Add(reader.GetDouble(1));
                }
            }
            // Execute command2

            double result = double.Parse(command2.ExecuteScalar().ToString());

            for (int i = 0; i < Temperatures.Count; i++)
            {
                Average.Add(new PindexData { Temperature = Temperatures.ElementAt(i), Pindex = result });
            }

            OnPropertyChanged(nameof(Data));
            OnPropertyChanged(nameof(Average));

            // Close database after execution
            conn.Close();
        }

        public ICommand RefreshCommand => new DelegateCommand((x) => new Task(RefreshChart).Start());

        #region Data Binding
        public DateTime StartDate
        {
            get { return _model.StartDate; }
            set
            {
                _model.StartDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime EndDate
        {
            get { return _model.EndDate; }
            set
            {
                _model.EndDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime FirstAvailableDate
        {
            get { return _model.FirstAvailableDate; }
            set
            {
                _model.FirstAvailableDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime LastAvailableDate
        {
            get { return _model.LastAvailableDate; }
            set
            {
                _model.LastAvailableDate = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}

