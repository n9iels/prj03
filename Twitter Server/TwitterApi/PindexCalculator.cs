using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Interfaces;

namespace TwitterApi
{
    class PindexCalculator : ITweetProcessor
    {
        public void Proces(ITweet tweet)
        {
            string inhoud = tweet.Text;
            double Pindex = Calculate(inhoud);

        }

        protected double Calculate(string tweettext)
        {
            double PIndex = 0;

            return PIndex;
        }

        protected void UploadToDatabase(ITweet tweet, double Pindex)
        {
            // Uploading to database implementation not available
        }
    }
}
