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
            BindingOperations.EnableCollectionSynchronization(Data, _dataLock);
            new Task(UpdateChart).Start();
        }

        public void UpdateChart()
        {
            using (ProjectEntities db = new ProjectEntities())
            {
                var res = from t in db.twitter_tweets
                              //where t.created_at > time_limit
                          group t by t.language
                    into lang
                          where lang.Count() > 75 && lang.Key != null && lang.Key != "UN_NotReferenced"
                          select new PieChartModel() { Category = lang.Key, Number = lang.Count() };
                Data.Clear();
                foreach (PieChartModel chart in res)
                {
                    Data.Add(chart);
                }
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