using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DataVisualization.Data.Models.PieChartModel;
using DataVisualization.Windows;

namespace DataVisualization.WindowsClient.ViewModels {
    public class PieChartViewModel : ViewModelBase {


        private readonly PieChartModel _model;

        public PieChartViewModel() {
            LanguageSelectCommand = new DelegateCommand(x => {
                _model.CurrentChartCommand = RefreshLanguages;
                RefreshCommand.Execute(null);
            });
            PositivitySelectCommand = new DelegateCommand(x => {
                _model.CurrentChartCommand = RefreshPositivity;
                RefreshCommand.Execute(null);
            });


            _model = new PieChartModel {
                CurrentChartCommand = RefreshLanguages
            };
            RefreshCommand.Execute(null);
        }

        public void RefreshLanguages() {
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
                    select new PieChartContent() { Category = entry.Language.Key, Number = entry.Language.Count() };

                var lowCount =
                    first.Select(
                        x =>
                            new PieChartContent() {
                                Category = "Other",
                                Number = first.Where(y => y.Count < sum * factor).Sum(z => z.Count)
                            });

                var res = highCount.Concat(lowCount);
                
                ChartData = new ObservableCollection<PieChartContent>(res);
            }

        }

        public void RefreshPositivity() {
            using (ProjectEntities db = new ProjectEntities()) {
                var res = from data in db.twitter_tweets
                    let range = (data.pindex < 0 ? "Negative" : data.pindex > 0 ? "Positive" : "Neutral")
                    group data by range
                    into r
                    select new PieChartContent() { Category = r.Key, Number = r.Count() };

                ChartData = new ObservableCollection<PieChartContent>(res);
            }
        }


        public ICommand RefreshCommand => new DelegateCommand((x) => new Task(_model.CurrentChartCommand).Start());

        public ICommand LanguageSelectCommand { get; }
        public ICommand PositivitySelectCommand { get; }

        public ObservableCollection<PieChartContent> ChartData
        {
            get { return _model.ChartData; }
            set
            {
                _model.ChartData = value;
                OnPropertyChanged();
            }
        }

        public object SelectedItem
        {
            get { return _model.SelectedChartItem; }
            set
            {
                _model.SelectedChartItem = value;
                OnPropertyChanged();
            }
        }
    }
}