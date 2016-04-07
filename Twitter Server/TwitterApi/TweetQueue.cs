using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Core.Interfaces;

namespace TwitterApi
{
    class TweetQueue
    {
        private Queue<ITweet> _queue =  new Queue<ITweet>();
        private ITweetProcessor _proces =  new PindexCalculator();

        //Adding tweet to the Queue
        public void Enqueue(ITweet tweet)
        {
            this._queue.Enqueue(tweet);
            ProcessQueue();

        }
        //Calling the process with a Tweet from the Queue
        private void ProcessQueue()
        {
            if (this._queue.Count() != 0)
            {
                ITweet tweet = this._queue.Dequeue();
                this._proces.Proces(tweet);
            }
        }
    }
}
