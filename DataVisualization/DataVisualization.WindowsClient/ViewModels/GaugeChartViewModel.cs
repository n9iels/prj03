using System.Collections.ObjectModel;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DataVisualization.Data.Models.GaugeChartModel;
using DataVisualization.Windows;
using MySql.Data.MySqlClient;

namespace DataVisualization.WindowsClient.ViewModels {
    class GaugeChartViewModel : ViewModelBase
    {
        public ObservableCollection<BarChartModel> ChartData { get; }
        private readonly object _lockObject = new object();

        public GaugeChartViewModel()
        {
            ChartData = new ObservableCollection<BarChartModel>();
            BindingOperations.EnableCollectionSynchronization(ChartData, _lockObject);
            RefreshCommand.Execute(null);
        }

        public void RefreshChart()
        {
            MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["dataBeest"].ConnectionString);
            conn.Open();
            MySqlCommand command1 = conn.CreateCommand();
            command1.CommandText =
                "SELECT SUM(case when tt.pindex <> 0 then 1 else 0 END) as Total, SUM(case when tt.pindex <> 0 AND tt.pindex > 0 then 1 else 0 END) as Positive, wt.name FROM twitter_tweets AS tt, weather_condition AS wc, weather_types AS wt WHERE tt.pindex != 0 AND wt.id = wc.id AND wc.date = tt.weather_date GROUP BY wt.name";

            MySqlDataReader reader = command1.ExecuteReader();
            while (reader.Read())
            {
                ChartData.Add(new BarChartModel() {
                    WeatherType = reader.GetString(2),
                    PositivityPercentage = (double)reader.GetInt64(1) / reader.GetInt64(0) * 100
                });

            }
            conn.Close();
            OnPropertyChanged(nameof(ChartData));
        }

        public ICommand RefreshCommand => new DelegateCommand((x) => new Task(RefreshChart).Start());
    }
}