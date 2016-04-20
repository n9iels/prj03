using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DataVisualization.Data.Models.LineGraphModel.LineGraphs;
using DataVisualization.Windows;
using MySql.Data.MySqlClient;

namespace DataVisualization.WindowsClient.ViewModels.LineGraphs {
    public class DayViewModel : ViewModelBase {
        public ObservableCollection<DayData> Data { get; private set; }
        private readonly DayModel _model;

        public DayViewModel() {
            _model = new DayModel();

            LastAvailableDate = DateTime.Now;
            StartDate = DateTime.Now - new TimeSpan(1, 0, 0, 0);
            EndDate = LastAvailableDate;

            new Task(() => {
                using (ProjectEntities db = new ProjectEntities()) {
                    var all = from row in db.twitter_tweets select row.created_at;
                    FirstAvailableDate = all.OrderBy(x => x).Take(1).Single();
                }
            }).Start();

            RefreshCommand.Execute(null);
        }

        public void RefreshChart() {
            MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["dataBeest"].ConnectionString);
            conn.Open();

            // Create SQL commands
            MySqlCommand command1 = conn.CreateCommand();
            command1.CommandText = "SELECT tt.created_at, AVG(tt.pindex) FROM twitter_tweets AS tt WHERE created_at >= @start AND created_at <= @end GROUP BY DATE(tt.created_at)";
            command1.Parameters.AddWithValue("@start", StartDate);
            command1.Parameters.AddWithValue("@end", EndDate);

            // Create lists and vars
            Data = new ObservableCollection<DayData>();

            // Execute command1
            using (MySqlDataReader reader = command1.ExecuteReader()) {
                while (reader.Read()) {
                    Data.Add(new DayData() { Date = reader.GetDateTime(0), Pindex = (double)reader.GetDecimal(1) });
                }
            }
            conn.Close();

            OnPropertyChanged(nameof(Data));
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