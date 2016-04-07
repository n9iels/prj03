using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Core.Interfaces;

namespace TwitterApi
{
    interface ITweetProcessor
    {
        void Proces(ITweet tweet);
    }
}
