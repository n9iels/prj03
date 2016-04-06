using System;
using TwitterApi;

namespace TwitterApi {

    internal class Program {

        private static void Main(string[] args) {


            TwitterStream stream = new TwitterStream();
            stream.Start();
            Console.ReadLine();

        }

    }
}