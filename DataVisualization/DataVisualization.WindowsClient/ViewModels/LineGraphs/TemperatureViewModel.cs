using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DataVisualization.Data.Models.LineGraphModel.LineGraphs;
using DataVisualization.Windows;

namespace DataVisualization.WindowsClient.ViewModels.LineGraphs {
    public class TemperatureViewModel : ViewModelBase {
        public ObservableCollection<TemperatureData> Data { get; private set; }
        private readonly TemperatureModel _model;

        public TemperatureViewModel() {
            _model = new TemperatureModel();

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

        public void RefreshChart() {
            using (ProjectEntities db = new ProjectEntities()) {
                var res = from wc in db.weather_condition
                    where wc.date >= StartDate && wc.date <= EndDate
                    group wc by wc.date
                    into conditions
                    select
                        new TemperatureData() {
                            Temperature = conditions.Average(c => c.temperaturegc),
                            Date = conditions.Key
                        };

                Data = new ObservableCollection<TemperatureData>(res);
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