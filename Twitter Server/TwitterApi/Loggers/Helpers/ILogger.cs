namespace TwitterApi.Loggers.Helpers {
    internal interface ILogger<T> {

        void Log(T data);
    }
}
