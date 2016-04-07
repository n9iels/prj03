using System;
using TwitterApi.Data_Processors;
using TwitterApi.Queues;
using TwitterApi.Streams;

namespace TwitterApi {

    internal class Program {

        private static void Main(string[] args) {


            TwitterStream stream = new TwitterStream(
                new TweetQueue(
                    new PositivityIndexCalculator()));

            stream.Start();
            Console.ReadLine();

        }

    }
}