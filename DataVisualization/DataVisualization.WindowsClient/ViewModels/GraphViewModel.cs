using System;
using System.Configuration;
using DataVisualization.Windows;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;
using DataVisualization.Data.Models.GraphModel;
using MySql.Data.MySqlClient;

namespace DataVisualization.WindowsClient.ViewModels
{
    public class GraphViewModel : ViewModelBase
    {

        private object _dataLock = new object();
        public ObservableCollection<PieChartModel> Data { get; private set; }

        public GraphViewModel()
        {
            Data = new ObservableCollection<PieChartModel>();
            //BindingOperations.EnableCollectionSynchronization(Data, _dataLock);
            new Task(UpdateChart).Start();
        }

        public void UpdateChart() {
            const double factor = 0.009;

            // Last 45 minutes
            DateTime time = DateTime.UtcNow - new TimeSpan(0, 0, 45, 0);

            using (ProjectEntities db = new ProjectEntities()) {
                var first = from t in db.twitter_tweets
                    //where t.created_at > time
                    group t by t.language
                    into lang
                    where lang.Key != null && lang.Key != "UN_NotReferenced"
                    select new { Language = lang, Count = lang.Count() };

                double sum = (from all in first select all.Count).Sum();


                var highCount = from entry in first
                    where entry.Count >= sum * factor
                    select new PieChartModel() { Category = entry.Language.Key, Number = entry.Language.Count() };


                var lowCount =
                    first.Select(
                        x =>
                            new PieChartModel() {
                                Category = "Other",
                                Number = first.Where(y => y.Count < sum * factor).Sum(z => z.Count)
                            });

                var res = highCount.Concat(lowCount);
                
                Data = new ObservableCollection<PieChartModel>(res);
                OnPropertyChanged(nameof(Data));
            }

        }


        public ICommand UpdateCommand => new DelegateCommand((x) => new Task(UpdateChart).Start());

        private object _selectedItem = null;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }
    }
}