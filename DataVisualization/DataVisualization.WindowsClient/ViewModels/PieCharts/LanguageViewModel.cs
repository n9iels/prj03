using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DataVisualization.Data.Models.PieChartModel.PieCharts;
using DataVisualization.Windows;

namespace DataVisualization.WindowsClient.ViewModels.PieCharts {
    public class LanguageViewModel : ViewModelBase {
        public ObservableCollection<LanguageData> Data { get; private set; }
        private readonly LanguageModel _model;

        public LanguageViewModel() {
            _model = new LanguageModel();

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
            const double factor = 0.009;

            using (ProjectEntities db = new ProjectEntities()) {

                var first = from t in db.twitter_tweets
                    where t.created_at > StartDate && t.created_at < EndDate
                    group t by t.language
                    into lang
                    where lang.Key != null && lang.Key != "UN_NotReferenced"
                    select new { Language = lang, Count = lang.Count() };

                double sum = (from all in first select all.Count).Sum();


                var highCount = from entry in first
                    where entry.Count >= sum * factor
                    select new LanguageData() { Category = entry.Language.Key, Number = entry.Language.Count() };

                var lowCount =
                    first.Select(
                        x =>
                            new LanguageData() {
                                Category = "Other",
                                Number = first.Where(y => y.Count < sum * factor).Sum(z => z.Count)
                            });

                var res = highCount.Concat(lowCount);

                Data = new ObservableCollection<LanguageData>(res);
                OnPropertyChanged(nameof(Data));
            }
        }

        public ICommand RefreshCommand => new DelegateCommand((x) => new Task(RefreshChart).Start());

        #region Data Binding

        public DateTime StartDate {
            get { return _model.StartDate; }
            set {
                _model.StartDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime EndDate {
            get { return _model.EndDate; }
            set {
                _model.EndDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime FirstAvailableDate {
            get { return _model.FirstAvailableDate; }
            set {
                _model.FirstAvailableDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime LastAvailableDate {
            get { return _model.LastAvailableDate; }
            set {
                _model.LastAvailableDate = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}