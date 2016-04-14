using Tweetinvi.Core.Interfaces;

namespace TwitterApi.Data_Processors.Helpers {
    internal interface IDataProcessor<in T> {
        void Process(T data);
    }
}
