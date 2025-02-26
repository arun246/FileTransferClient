using FileTransferClient.Application.Contracts;
using System;

namespace FileTransferClient.Infrastructure.Logging
{
    public class EventLoggerImplementation : ILogger
    {
        public void LogInfo(string message) => EventLogger.LogInfo(message);
        public void LogWarning(string message) => EventLogger.LogWarning(message);
        public void LogError(string message, Exception ex = null) => EventLogger.LogError(message, ex);
    }
}
