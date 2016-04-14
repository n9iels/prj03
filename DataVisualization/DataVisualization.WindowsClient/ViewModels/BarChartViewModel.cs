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
                //var daaat = (from r in db.weather_condition from x in db.twitter_tweets where r.date < x.created_at orderby r.date descending select r.date).First();
                //var shit = (from r in db.weather_condition select r.date);
                var first = from x in db.twitter_tweets
                            from r in db.weather_condition
                            from wt in db.weather_types
                            where r.id == wt.id
                            where r.date == (from r in db.weather_condition from x in db.twitter_tweets where r.date < x.created_at orderby r.date descending select r.date).FirstOrDefault()
                            group wt by wt.name
                            into tweets
                            select new BarChartModel { Positief = tweets.Key, Count = (from z in db.twitter_tweets select z.pindex).Average()};


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