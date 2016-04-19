using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DataVisualization.Data.Models.PieChartModel.PieCharts;
using DataVisualization.Windows;

namespace DataVisualization.WindowsClient.ViewModels.PieCharts {
    public class PositivityViewModel : ViewModelBase
    {
        public ObservableCollection<PositivityData> Data { get; private set; }
        private readonly PositivityModel _model;

        public PositivityViewModel() {
            _model = new PositivityModel();

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

        public void RefreshChart()
        {
            using (ProjectEntities db = new ProjectEntities())
            {
                var res = from data in db.twitter_tweets
                          where data.created_at > StartDate && data.created_at < EndDate
                          let range = (data.pindex < 0 ? "Negative" : data.pindex > 0 ? "Positive" : "Neutral")
                          group data by range
                    into r
                          select new PositivityData() { Category = r.Key, Number = r.Count() };

                Data = new ObservableCollection<PositivityData>(res);
                OnPropertyChanged(nameof(Data));
            }
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
