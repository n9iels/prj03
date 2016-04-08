using Tweetinvi.Core.Interfaces;

namespace TwitterApi.Loggers.Log_Containers {
    internal class TweetLogData {
        internal ITweet Tweet { get; }
        internal double PIndex { get; }

        public TweetLogData(ITweet tweet, double pIndex) {
            Tweet = tweet;
            PIndex = pIndex;
        }

    }
}
