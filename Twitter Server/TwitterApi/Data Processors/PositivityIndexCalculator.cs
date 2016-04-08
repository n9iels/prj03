﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MySql.Data.MySqlClient;
using Tweetinvi.Core.Interfaces;
using TwitterApi.Data_Processors.Helpers;
using TwitterApi.Data_Processors.MatchFinder.Helpers;
using TwitterApi.Loggers;
using TwitterApi.Loggers.Helpers;

namespace TwitterApi.Data_Processors {
    internal class PositivityIndexCalculator : IDataProcessor<ITweet> {
        private readonly IMatchFinder _matchFinder;
        private readonly ILogger<TweetLogData> _logger; 

        public PositivityIndexCalculator(IMatchFinder matchFinder, ILogger<TweetLogData> logger) {
            _matchFinder = matchFinder;
            _logger = logger;
        }

        public void Process(ITweet tweet) {
            double pIndex = Calculate(tweet.Text);
            UploadToDatabase(tweet, pIndex);
        }

        protected double Calculate(string tweetText) {
            Dictionary<string, int> positiveWords = _matchFinder.FindMatches(tweetText, "Word Lists/Positive-Words-NL.txt", "Word Lists/Positive-Words-EN.txt");
            Dictionary<string, int> negativeWords = _matchFinder.FindMatches(tweetText, "Word Lists/Negative-Words-NL.txt" , "Word Lists/Negative-Words-EN.txt");

            double positive = positiveWords.Sum(x => Math.Pow(x.Value, 0.5));
            double negative = negativeWords.Sum(x => Math.Pow(x.Value, 0.5));
            return positive - negative;
        }

        protected void UploadToDatabase(ITweet tweet, double pIndex) {
            using (
                MySqlConnection conn =
                    new MySqlConnection(ConfigurationManager.ConnectionStrings["dataBeest"].ConnectionString)) {
                conn.Open();
                MySqlCommand com = MySqlQueryGenerator.GenerateQuery(conn,
                    "INSERT INTO twitter_tweets VALUES(@id,@created,@profileId,@placeId,@text,@coordLong,@coordLat,@pIndex);",
                    tweet.Id, tweet.CreatedAt, tweet.CreatedBy.Id, tweet.Place.IdStr, tweet.Text,
                    tweet.Coordinates.Longitude, tweet.Coordinates.Latitude, pIndex);
                com.ExecuteNonQuery();
            }
            _logger.Log(new TweetLogData(tweet, pIndex));
        }
    }
}
