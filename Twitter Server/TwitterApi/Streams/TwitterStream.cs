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
        
        /// <summary>
        /// Gets the <see cref="FilteredStream"/> used in the <see cref="TwitterStream"/>.
        /// </summary>
        internal FilteredStream Stream { get; }

        private readonly QueueBase<ITweet> _queue;

        public event EventHandler<MatchedTweetReceivedEventArgs> TweetReceived {
            add { Stream.TweetReceived += value; }
            remove { Stream.TweetReceived -= value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterStream"/> class using the specified <see cref="QueueBase{T}"/>.
        /// </summary>
        /// <param name="queue"></param>
        public TwitterStream(QueueBase<ITweet> queue) {
            _queue = queue;

            // Authentication
            Auth.SetUserCredentials(Properties.Settings.Default.OauthConsumerKey,
                Properties.Settings.Default.OauthConsumerSecret, Properties.Settings.Default.OauthToken,
                Properties.Settings.Default.OauthTokenSecret);

            // Creating the stream
            Stream = (FilteredStream) Tweetinvi.Stream.CreateFilteredStream();
        }

        /// <summary>
        /// Starts the <see cref="TwitterStream"/>.
        /// </summary>
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
            //Enqueue the tweet
            _queue.Enqueue(e.Tweet);

        }
    }
}
