using System.Globalization;
using System.Threading;
using TwitterApi.Data_Processors;
using TwitterApi.Data_Processors.MatchFinder;
using TwitterApi.Loggers;
using TwitterApi.Queues;
using TwitterApi.Streams;

namespace TwitterApi {

    internal class Program {

        private static void Main() {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;


            // SOLID
            // [D]ependency inversion principle.

            TwitterStream stream = new TwitterStream(
                new TweetQueue(
                    new PositivityIndexCalculator(
                        new TxtNewLineMatchFinder(), new TweetLogger())));
            stream.Start();
        }

    }
}