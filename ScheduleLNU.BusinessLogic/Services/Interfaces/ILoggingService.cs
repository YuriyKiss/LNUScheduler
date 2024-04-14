namespace ScheduleLNU.BusinessLogic.Services.Interfaces
{
    public interface ILoggingService<T>
    {
        public void LogTrace(string message, params object[] args);

        public void LogInfo(string message, params object[] args);

        public void LogWarning(string message, params object[] args);

        public void LogDebug(string message, params object[] args);

        public void LogError(string message, params object[] args);

        public void LogCritical(string message, params object[] args);
    }
}
