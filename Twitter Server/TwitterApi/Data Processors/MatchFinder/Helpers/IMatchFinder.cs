using System.Collections.Generic;

namespace TwitterApi.Data_Processors.MatchFinder.Helpers {
    internal interface IMatchFinder {
        Dictionary<string, int> FindMatches(string data, params string[] filePaths);
    }
}
