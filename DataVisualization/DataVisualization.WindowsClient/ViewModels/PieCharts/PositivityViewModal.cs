using DataVisualization.Data.Models.PieChartModel.PieCharts;
using DataVisualization.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DataVisualization.WindowsClient.ViewModels.PieCharts
{
    public class PositivityViewModal : ViewModelBase
    {
        public ObservableCollection<PositivityModel> Data { get; private set; }

        public PositivityViewModal()
        {
            RefreshCommand.Execute(null);
        }

        public void RefreshChart()
        {
            using (ProjectEntities db = new ProjectEntities())
            {
                var res = from data in db.twitter_tweets
                          let range = (data.pindex < 0 ? "Negative" : data.pindex > 0 ? "Positive" : "Neutral")
                          group data by range
                    into r
                          select new PositivityModel() { Category = r.Key, Number = r.Count() };

                Data = new ObservableCollection<PositivityModel>(res);
                OnPropertyChanged(nameof(Data));
            }
        }

        public ICommand RefreshCommand => new DelegateCommand((x) => new Task(RefreshChart).Start());
    }
}
