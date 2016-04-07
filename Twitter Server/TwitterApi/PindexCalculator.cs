using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Tweetinvi.Core.Interfaces;
using MySql.Data.MySqlClient;

namespace TwitterApi
{
    class PindexCalculator : ITweetProcessor
    {
        private double x = 1;
        public void Proces(ITweet tweet)
        {
            string inhoud = tweet.Text;
            double Pindex = Calculate(inhoud);
            UploadToDatabase(tweet, Pindex);

        }

        protected double Calculate(string tweettext)
        {
            double PIndex = x;
            string[] woorden = tweettext.Split();
            foreach (string e in woorden)
            {
                   
            }
            return PIndex;
        }

        protected void UploadToDatabase(ITweet tweet, double Pindex)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["dataBeest"].ConnectionString;
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            Console.WriteLine(tweet.Text + " " + Pindex);
        }
    }
}
