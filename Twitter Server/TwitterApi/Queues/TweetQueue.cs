using Tweetinvi.Core.Interfaces;
using TwitterApi.Data_Processors.Helpers;
using TwitterApi.Queues.Helpers;

namespace TwitterApi.Queues
{
    internal class TweetQueue : QueueBase<ITweet>
    {
        private readonly IDataProcessor<ITweet> _processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetQueue"/> class using the specified <see cref="IDataProcessor{T}"/>
        /// </summary>
        /// <param name="processor"></param>
        public TweetQueue(IDataProcessor<ITweet> processor) {
            _processor = processor;
        }

        /// <summary>
        /// Processes the specified <see cref="ITweet"/>.
        /// </summary>
        /// <param name="data">The <see cref="ITweet"/> to be processed.</param>
        protected override void ProcessData(ITweet data) {
            _processor.Process(data);
        }
    }
}
