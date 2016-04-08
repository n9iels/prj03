using System;
using Tweetinvi.Core.Interfaces;
using TwitterApi.Loggers.Helpers;

namespace TwitterApi.Loggers {
    internal class TweetLogger : ILogger<TweetLogData> {
        public void Log(TweetLogData data) {
            Console.WriteLine(data.Tweet.Text);
            Console.Write("Has index : ");
            if (data.PIndex < 0)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (data.PIndex > 0)
                Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(data.PIndex);
            Console.WriteLine();
            Console.ResetColor();
        }
    }

    internal class TweetLogData {
        internal ITweet Tweet { get; }
        internal double PIndex { get; }

        public TweetLogData(ITweet tweet, double pIndex) {
            Tweet = tweet;
            PIndex = pIndex;
        }
        
    }
}
