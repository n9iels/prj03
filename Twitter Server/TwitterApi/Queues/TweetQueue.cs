using Tweetinvi.Core.Interfaces;
using TwitterApi.Data_Processors.Helpers;
using TwitterApi.Queues.Helpers;

namespace TwitterApi.Queues
{
    internal class TweetQueue : QueueBase<ITweet>
    {
        private readonly ITweetProcessor _processor;

        public TweetQueue(ITweetProcessor processor) {
            _processor = processor;
        }

        protected override void ProcessData(ITweet data) {
            _processor.Process(data);
        }
    }
}
