using System;
using Tweetinvi;
using Tweetinvi.Core.Events.EventArguments;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Core.Interfaces.Models;
using Tweetinvi.Core.Parameters;
using Tweetinvi.Streams;
using TwitterApi.Queues.Helpers;

namespace TwitterApi.Streams {

    internal class TwitterStream {
        
        internal FilteredStream Stream { get; }

        private readonly QueueBase<ITweet> _queue;

        public event EventHandler<MatchedTweetReceivedEventArgs> TweetReceived {
            add { Stream.TweetReceived += value; }
            remove { Stream.TweetReceived -= value; }
        }

        public TwitterStream(QueueBase<ITweet> queue) {
            _queue = queue;

            // Authentication
            Auth.SetUserCredentials(Properties.Settings.Default.OauthConsumerKey,
                Properties.Settings.Default.OauthConsumerSecret, Properties.Settings.Default.OauthToken,
                Properties.Settings.Default.OauthTokenSecret);

            // Creating the stream
            Stream = (FilteredStream) Tweetinvi.Stream.CreateFilteredStream();
        }

        public void Start() {
            // Start listening to TweetReceived event using the Received method.
            Stream.MatchingTweetReceived += Received;

            // GeoCoordinates Rotterdam
            ICoordinates sw = new Coordinates(4.325111, 51.854432);
            ICoordinates ne = new Coordinates(4.667061, 51.950285);
            
            Stream.AddLocation(sw, ne);
            Stream.StartStreamMatchingAllConditions();

        }

        private void Received(object sender, MatchedTweetReceivedEventArgs e) {

            // Main information needed for database
            long id = e.Tweet.Id;
            string name = e.Tweet.CreatedBy.Name;
            string text = e.Tweet.Text;
            DateTime created = e.Tweet.CreatedAt;
            ICoordinates coords = e.Tweet.Coordinates;
            IPlace place = e.Tweet.Place;

            // Information for potential future us.
            int retweets = e.Tweet.RetweetCount;
            int favouriteCount = e.Tweet.FavoriteCount;

            //Enqueue the tweet
            _queue.Enqueue(e.Tweet);

        }
    }
}
