using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataVisualization.Data.Models.BarChartModel;
using DataVisualization.Windows;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;

namespace DataVisualization.WindowsClient.ViewModels
{
    class BarChartViewModel : ViewModelBase
    {
        public ObservableCollection<BarChartModel> bier { get; private set; }

        public BarChartViewModel()
        {
            bier = new ObservableCollection<BarChartModel>();
            UpdateChart();

        }

        public void UpdateChart()
        {
            List<double> alles = new List<double>();
            int hallo = 0;
            string connectionString = "SERVER=daniel-molenaar.com;" + "DATABASE=danielmo_project3;" +"UID=danielmo_pr3Clnt;" + "PASSWORD=-*AGq=3wA_aJV=V;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            MySqlCommand command1 = conn.CreateCommand();
            MySqlCommand command2 = conn.CreateCommand();
            command1.CommandText = "SELECT COUNT(tt.id), wt.name FROM twitter_tweets AS tt, weather_condition AS wc, weather_types AS wt WHERE tt.pindex != 0 AND wt.id = wc.id AND wc.date = (SELECT date FROM weather_condition WHERE date < tt.created_at ORDER BY date DESC LIMIT 1) GROUP BY wt.name";
            command2.CommandText = "SELECT COUNT(tt.id), wt.name FROM twitter_tweets AS tt, weather_condition AS wc, weather_types AS wt WHERE tt.pindex != 0 AND tt.pindex > 0 AND wt.id = wc.id AND wc.date = (SELECT date FROM weather_condition WHERE date < tt.created_at ORDER BY date DESC LIMIT 1) GROUP BY wt.name";
            MySqlDataReader Reader = command1.ExecuteReader();
            while (Reader.Read())
            {
                alles.Add((Reader.GetDouble(0)/100));  
            }
            conn.Close();
            conn.Open();
            Reader = command2.ExecuteReader();
            while (Reader.Read())
            {
                double perc = Reader.GetDouble(0) / alles[hallo];
                hallo += 1;
                var hond = new BarChartModel { Count = perc,Positief = Reader.GetString(1) };
                bier.Add(hond);
            }
            conn.Close();
            OnPropertyChanged(nameof(bier));



            //           // Connect to the database
            //+string connectionString = ConfigurationManager.ConnectionStrings["dataBeest"].ConnectionString;
            //           +conn = new MySqlConnection(connectionString);
            //           +conn.Open();
            //           +
            //           +            // Create SQL command
            //           +MySqlCommand command = conn.CreateCommand();
            //           +MySqlDataReader Reader;
            //           +command.CommandText = "SELECT * FROM weather_condition";
            //           +
            //           +            // Read result
            //           +Reader = command.ExecuteReader();
            //           +            while (Reader.Read())
            //               +            {
            //               +Console.WriteLine(Reader.GetValue(1).ToString());
            //               +            }
            //           +conn.Close();

            //using (ProjectEntities db = new ProjectEntities())
            //{
            //    //var daaat = (from r in db.weather_condition from x in db.twitter_tweets where r.date < x.created_at orderby r.date descending select r.date).First();
            //    //var shit = (from r in db.weather_condition select r.date);
            //    //var first = from x in db.twitter_tweets
            //    //            from r in db.weather_condition
            //    //            from wt in db.weather_types
            //    //            where r.id == wt.id
            //    //            where r.date == (from r in db.weather_condition from x in db.twitter_tweets where r.date < x.created_at orderby r.date descending select r.date).FirstOrDefault()
            //    //            group wt by wt.name
            //    //            into tweets
            //    //            select new BarChartModel { Positief = tweets.Key, Count = (from z in db.twitter_tweets select z.pindex).Average()};



            //    //var allp = (from x in db.twitter_tweets
            //    //            from r in db.weather_condition
            //    //            from wt in db.weather_types
            //    //            where r.id == wt.id
            //    //            where r.date == (from r in db.weather_condition from x in db.twitter_tweets where r.date < x.created_at orderby r.date descending select r.date).FirstOrDefault()
            //    //            where x.pindex != 0
            //    //            group wt by wt.name
            //    //           into iets
            //    //            select iets.Key);

            //    //double allesp = allp.Count();


            //    //int pos = (from x in db.twitter_tweets
            //    //           where x.pindex != 0 && x.pindex > 0
            //    //           select x).Count();

            //    //int een = all / 100;

            //    //double avg = pos / een;


            //    //var first = from x in db.weather_types
            //    //            select new BarChartModel { Count = avg, Positief = "algemeen" };


            //    //SELECT wt.name AS Weather, AVG(tt.pindex) AS Average_Positivity
            //    //FROM weather_condition AS wc, weather_types AS wt, twitter_tweets AS tt
            //    //WHERE wc.id = wt.id
            //    //AND wc.date = (SELECT date FROM weather_condition WHERE date < tt.created_at ORDER BY date DESC LIMIT 1)
            //    //GROUP BY wt.name




            //    var first = from  x in db.twitter_places
            //                select new BarChartModel { Count = 100, Positief = "Aids" };

            //    bier = new ObservableCollection<BarChartModel>(all);
            //    OnPropertyChanged(nameof(bier));
            //}
        }
    }
}