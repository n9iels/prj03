using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataVisualization.Data.Models.BarChartModel;
using DataVisualization.Windows;
using System.Collections.ObjectModel;

namespace DataVisualization.WindowsClient.ViewModels
{
    class BarChartViewModel : ViewModelBase
    {
        public ObservableCollection<BarChartModel> bier { get; private set; }

        public BarChartViewModel()
        {
            bier = new ObservableCollection<BarChartModel>();
            new Task(UpdateChart).Start();

        }

        public void UpdateChart()
        {

            using (ProjectEntities db = new ProjectEntities())
            {
                var first = from x in db.weather_types
                            select new BarChartModel { Count = 69, Positief = x.name };

                //SELECT wt.name AS Weather, AVG(tt.pindex) AS Average_Positivity
                //FROM weather_condition AS wc, weather_types AS wt, twitter_tweets AS tt
                //WHERE wc.id = wt.id
                //AND wc.date = (SELECT date FROM weather_condition WHERE date < tt.created_at ORDER BY date DESC LIMIT 1)
                //GROUP BY wt.name






                bier = new ObservableCollection<BarChartModel>(first);
                OnPropertyChanged(nameof(bier));
            }
        }
    }
}