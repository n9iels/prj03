using System;
using TwitterApi.Loggers.Helpers;
using TwitterApi.Loggers.Log_Containers;

namespace TwitterApi.Loggers {
    internal class TweetLogger : ILogger<TweetLogData> {

        /// <summary>
        /// Logs the specified <see cref="TweetLogData"/> to the <see cref="Console"/>.
        /// </summary>
        /// <param name="data"></param>
        public void Log(TweetLogData data) {
            Console.WriteLine(data.Tweet.Text);
            Console.Write("Has index : ");
            if (data.PIndex < 0)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (data.PIndex > 0)
                Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(data.PIndex);
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
