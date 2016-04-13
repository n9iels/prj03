using DataVisualization.Data.Models.LineGraphModel;
using DataVisualization.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DataVisualization.WindowsClient.ViewModels
{
    public class LineGraphViewModel : ViewModelBase
    {
        public ObservableCollection<LineGraphModel> Data { get; private set; }

        public LineGraphViewModel()
        {
            Data = new ObservableCollection<LineGraphModel>();
            RefreshCommand.Execute(null);
        }

        public void RefreshChart()
        {
            using (ProjectEntities db = new ProjectEntities())
            {
                var res = from wc in db.weather_condition
                            group wc by wc.date into conditions
                            select new LineGraphModel() { Temperature = conditions.Average(c => c.temperaturegc), Date = conditions.Key };

                Data = new ObservableCollection<LineGraphModel>(res);
                OnPropertyChanged(nameof(Data));
            }
        }

        public ICommand RefreshCommand => new DelegateCommand((x) => new Task(RefreshChart).Start());
    }
}
