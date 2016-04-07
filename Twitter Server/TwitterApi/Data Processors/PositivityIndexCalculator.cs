using System;
using Tweetinvi.Core.Interfaces;
using TwitterApi.Data_Processors.Helpers;

namespace TwitterApi.Data_Processors {
    internal class PositivityIndexCalculator : ITweetProcessor {
        public void Process(ITweet tweet) {
            double pIndex = Calculate(tweet.Text);
            UploadToDatabase(tweet, pIndex);
        }

        protected double Calculate(string tweettext) {
            string[] woorden = tweettext.Split();
            foreach (string e in woorden) { }
            return 0;
        }

        protected void UploadToDatabase(ITweet tweet, double pIndex) {
            //string connectionString = ConfigurationManager.ConnectionStrings["dataBeest"].ConnectionString;
            //MySqlConnection conn = new MySqlConnection(connectionString);
            //conn.Open();
            Console.WriteLine(tweet.Text + " " + pIndex);
        }
    }
}
