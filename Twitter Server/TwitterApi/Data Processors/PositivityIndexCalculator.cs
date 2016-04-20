using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using MySql.Data.MySqlClient;
using Tweetinvi.Core.Interfaces;
using TwitterApi.Data_Processors.Helpers;
using TwitterApi.Data_Processors.MatchFinder.Helpers;
using TwitterApi.Loggers.Helpers;
using TwitterApi.Loggers.Log_Containers;

namespace TwitterApi.Data_Processors {
    internal class PositivityIndexCalculator : IDataProcessor<ITweet> {
        private readonly IMatchFinder _matchFinder;
        private readonly ILogger<TweetLogData> _logger; 

        /// <summary>
        /// Initializes a new instance of the <see cref="PositivityIndexCalculator"/> class using the specified <see cref="IMatchFinder"/> and <see cref="ILogger{TweetLogData}"/>.
        /// </summary>
        /// <param name="matchFinder">The <see cref="IMatchFinder"/> to use.</param>
        /// <param name="logger">The <see cref="ILogger{T}"/> to use.</param>
        public PositivityIndexCalculator(IMatchFinder matchFinder, ILogger<TweetLogData> logger) {
            _matchFinder = matchFinder;
            _logger = logger;
        }

        /// <summary>
        /// Processes a <see cref="ITweet"/>.
        /// </summary>
        /// <param name="tweet">The <see cref="ITweet"/> to be processed.</param>
        public void Process(ITweet tweet) {
            double pIndex = Calculate(tweet.Text);
            UploadToDatabase(tweet, pIndex);
        }

        /// <summary>
        /// Calculates the pindex using the specified text.
        /// </summary>
        /// <param name="tweetText">The text used in the pindex calculation.</param>
        /// <returns>A <see cref="double"/> containing the pindex.</returns>
        protected double Calculate(string tweetText) {
            Dictionary<string, int> positiveWords = _matchFinder.FindMatches(tweetText, "Resources/Word Lists/Positive-Words-NL.txt", "Resources/Word Lists/Positive-Words-EN.txt");
            Dictionary<string, int> negativeWords = _matchFinder.FindMatches(tweetText, "Resources/Word Lists/Negative-Words-NL.txt" , "Resources/Word Lists/Negative-Words-EN.txt");

            double positive = positiveWords.Sum(x => Math.Pow(x.Value, 0.5));
            double negative = negativeWords.Sum(x => Math.Pow(x.Value, 0.5));
            return positive - negative;
        }

        /// <summary>
        /// Uploads the specifed <see cref="ITweet"/> data and the specified pindex to the database.
        /// </summary>
        /// <param name="tweet">The <see cref="ITweet"/> to upload.</param>
        /// <param name="pIndex">The pindex to upload.</param>
        protected void UploadToDatabase(ITweet tweet, double pIndex) {
            try {
                using (
                    MySqlConnection conn =
                        new MySqlConnection(ConfigurationManager.ConnectionStrings["dataBeest"].ConnectionString)) {
                    conn.Open();
                    MySqlCommand com = MySqlQueryGenerator.GenerateQuery(conn,
                        "INSERT INTO twitter_tweets(SELECT @id, @created, @profileId, @placeId, @text, @coordLong, @coordLat, @pIndex, @language, date FROM weather_condition WHERE date < @createdAt ORDER BY date DESC LIMIT 1)",
                        tweet.Id, tweet.CreatedAt, tweet.CreatedBy.Id, tweet.Place?.IdStr, tweet.Text,
                        tweet.Coordinates?.Longitude, tweet.Coordinates?.Latitude, pIndex, tweet.Language.ToString(), tweet.CreatedAt);
                    com.ExecuteNonQuery();

                    if (tweet.Place != null) {
                        MySqlCommand comPlaces = MySqlQueryGenerator.GenerateQuery(conn,
                            "INSERT IGNORE INTO twitter_places VALUES(@id,@name,@fullName)", tweet.Place.IdStr,
                            tweet.Place.Name, tweet.Place.FullName);
                        comPlaces.ExecuteNonQuery();
                    }

                    MySqlCommand comProfiles = MySqlQueryGenerator.GenerateQuery(conn,
                        "INSERT IGNORE INTO twitter_profiles VALUES (@id,@name,@screenName,@profileImage);",
                        tweet.CreatedBy.Id, tweet.CreatedBy.Name, tweet.CreatedBy.ScreenName,
                        tweet.CreatedBy.ProfileImageUrl);
                    comProfiles.ExecuteNonQuery();
                }
                _logger.Log(new TweetLogData(tweet, pIndex));
            }
            catch (MySqlException ex) {
                string[] lines = {
                    "=================",
                    $"Error at time (DateTime.Now) : {DateTime.Now}",
                    $"Error at message : {tweet.Text}",
                    $"Error log : {ex.Message}",
                    "",
                    ""
                };
                try {
                    File.AppendAllLines("error.log", lines);
                }
                catch { }
            }
        }
    }
}
