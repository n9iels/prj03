using DataVisualization.Data.Models.LineGraphModel.LineGraphs;
using DataVisualization.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DataVisualization.WindowsClient.ViewModels.LineGraphs
{
    class TemperatureViewModel : ViewModelBase
    {
        public ObservableCollection<TemperatureModel> Data { get; private set; }

        public TemperatureViewModel()
        {
            RefreshCommand.Execute(null);
        }

        public void RefreshChart()
        {
            using (ProjectEntities db = new ProjectEntities())
            {
                var res = from wc in db.weather_condition
                          group wc by wc.date into conditions
                          select new TemperatureModel() { Temperature = conditions.Average(c => c.temperaturegc), Date = conditions.Key };

                Data = new ObservableCollection<TemperatureModel>(res);
                OnPropertyChanged(nameof(Data));
            }
        }

        public ICommand RefreshCommand => new DelegateCommand((x) => new Task(RefreshChart).Start());
    }
}
