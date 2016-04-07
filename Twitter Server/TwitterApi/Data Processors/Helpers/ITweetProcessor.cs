using Tweetinvi.Core.Interfaces;

namespace TwitterApi.Data_Processors.Helpers {
    internal interface ITweetProcessor {
        void Process(ITweet tweet);
    }
}
