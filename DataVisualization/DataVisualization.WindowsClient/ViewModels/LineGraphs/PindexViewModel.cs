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
    class PindexViewModel : ViewModelBase
    {
        public ObservableCollection<PindexModel> Data { get; private set; }

        public PindexViewModel()
        {
            RefreshCommand.Execute(null);
        }

        public void RefreshChart()
        {
            using (ProjectEntities db = new ProjectEntities())
            {
                var res = from wc in db.weather_condition
                          join tt in db.twitter_tweets on wc.date equals (from r in db.weather_condition from x in db.twitter_tweets where r.date < x.created_at orderby r.date descending select r.date).FirstOrDefault()
                          group wc by wc.temperaturegc into conditions
                          select new PindexModel() { Temperature = conditions.Average(c => c.temperaturegc), Pindex = tt.pindex};

                Data = new ObservableCollection<PindexModel>(res);
                OnPropertyChanged(nameof(Data));
            }
        }

        public ICommand RefreshCommand => new DelegateCommand((x) => new Task(RefreshChart).Start());
    }
}

