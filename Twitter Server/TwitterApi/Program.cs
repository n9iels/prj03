using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using TwitterApi.Data_Processors;
using TwitterApi.Data_Processors.Helpers;
using TwitterApi.Data_Processors.MatchFinder;
using TwitterApi.Data_Processors.MatchFinder.Helpers;
using TwitterApi.Loggers;
using TwitterApi.Queues;
using TwitterApi.Queues.Helpers;
using TwitterApi.Streams;

namespace TwitterApi {

    internal class Program {

        private static void Main(string[] args) {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            TwitterStream stream = new TwitterStream(
                new TweetQueue(
                    new PositivityIndexCalculator(
                        new TxtNewLineMatchFinder(), new TweetLogger())));

            stream.Start();
        }

    }

    class TestQueue : QueueBase<string> {
        private readonly IDataProcessor<string> _processor;


        public TestQueue(IDataProcessor<string> processor) {
            _processor = processor;
        }
        protected override void ProcessData(string data) {
            _processor.Process(data);
        }
    }

    class Processor : IDataProcessor<String> {
        private readonly IMatchFinder _match;
        public Processor(IMatchFinder match) {
            _match = match;
        }
        public void Process(string data) {
            double result = Calculate(data);
            Console.WriteLine(data);
            Console.WriteLine("Index : " + result);

        }

        protected double Calculate(string tweetText) {
            Dictionary<string, int> positiveWords = _match.FindMatches(tweetText, "Word Lists/Positive-Words-NL.txt", "Word Lists/Positive-Words-EN.txt");
            Dictionary<string, int> negativeWords = _match.FindMatches(tweetText, "Word Lists/Negative-Words-NL.txt", "Word Lists/Negative-Words-EN.txt");

            double positive = positiveWords.Sum(x => Math.Pow(x.Value, 0.5));
            double negative = negativeWords.Sum(x => Math.Pow(x.Value, 0.5));
            return positive - negative;
        }
    }
}