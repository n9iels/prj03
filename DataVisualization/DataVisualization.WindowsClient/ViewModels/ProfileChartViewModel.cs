using DataVisualization.Windows;
using DataVisualization.Data.Models.ProfileChartModel;
using DataVisualization.Data.Models.PieChartModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace DataVisualization.WindowsClient.ViewModels
{
    public class ProfileChartViewModel : ViewModelBase
    {
        public ObservableCollection<ProfileChartModel> profiles { get; private set; }
        private object _lockObject = new object();

        public ProfileChartViewModel()
        {
            profiles = new ObservableCollection<ProfileChartModel>();
            BindingOperations.EnableCollectionSynchronization(profiles, _lockObject);
            RefreshCommand.Execute(null);
        }

        public void RefreshChart()
        {
            MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["dataBeest"].ConnectionString);
            conn.Open();

            MySqlCommand command = conn.CreateCommand();
            command.CommandText = "SELECT COUNT(tt.id) AS hoi, tp.name FROM twitter_tweets AS tt, twitter_profiles AS tp WHERE tt.profile_id = tp.id GROUP BY tt.profile_id ORDER BY hoi DESC LIMIT 10";
            //command.CommandText = "SELECT COUNT(tt.id), wt.name FROM twitter_tweets AS tt, weather_condition AS wc, weather_types AS wt WHERE tt.pindex != 0 AND wt.id = wc.id AND wc.date = (SELECT date FROM weather_condition WHERE date < tt.created_at ORDER BY date DESC LIMIT 1) GROUP BY wt.name";


            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var pils = new ProfileChartModel { tweets = reader.GetInt32(0), name = reader.GetString(1) };
                    profiles.Add(pils);
                }
            }
            conn.Close();
            OnPropertyChanged(nameof(profiles));
        }
        public ICommand RefreshCommand => new DelegateCommand((x) => new Task(RefreshChart).Start());
    }
}
