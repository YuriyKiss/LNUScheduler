using System;
using Microsoft.Extensions.Logging;
using ScheduleLNU.BusinessLogic.Services.Interfaces;

namespace ScheduleLNU.BusinessLogic.Services
{
    public class LogginService<T> : ILoggingService<T>
    {
        private readonly ILogger<T> logger;

        public LogginService(ILogger<T> logger)
        {
            this.logger = logger;
        }

        public void LogCritical(string message, params object[] args)
        {
            logger.LogCritical(message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            logger.LogDebug(message, args);
        }

        public void LogError(string message, params object[] args)
        {
            logger.LogError(message, args);
        }

        public void LogInfo(string message, params object[] args)
        {
            logger.LogInformation(message, args);
        }

        public void LogTrace(string message, params object[] args)
        {
            logger.LogTrace(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            logger.LogWarning(message, args);
        }
    }
}
