using System;
using Serilog;

namespace FileTransferClient.Infrastructure.Logging
{
    public static class EventLogger
    {
        private static ILogger _logger;
        private static Action<string> _logAction;

        static EventLogger()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Console() // This requires the Serilog.Sinks.Console package
                .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day) // This requires the Serilog.Sinks.File package
                .CreateLogger();
        }

        public static void Initialize(Action<string> logAction)
        {
            _logAction = logAction;
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        public static void LogInfo(string message)
        {
            _logger.Information(message);
            _logAction?.Invoke($"INFO: {message}");
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        public static void LogWarning(string message)
        {
            _logger.Warning(message);
            _logAction?.Invoke($"WARNING: {message}");
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        public static void LogError(string message, Exception ex = null)
        {
            _logger.Error(ex, message);
            _logAction?.Invoke($"ERROR: {message} - {ex?.Message}");
        }
    }
}