using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwitterApi.Data_Processors.MatchFinder.Helpers;

namespace TwitterApi.Data_Processors.MatchFinder {
    internal class TxtNewLineMatchFinder : IMatchFinder {
        public Dictionary<string, int> FindMatches(string data, params string[] filePaths) {
            var matches = new Dictionary<string, int>();
            string[] lower = data.ToLowerInvariant().Split();

            foreach (string path in filePaths) {
                using (StreamReader stream = new StreamReader(path)) {
                    string line;
                    while (!string.IsNullOrWhiteSpace(line = stream.ReadLine())) {
                        int match = lower.Count(x => x.Contains(line));
                        if(match > 0 && !matches.ContainsKey(line))
                            matches.Add(line, match);
                    }
                }
            }
            return matches;
        }
    }
}
