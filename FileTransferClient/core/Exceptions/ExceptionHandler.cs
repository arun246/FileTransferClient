using System;
using System.Windows.Forms;
using FileTransferClient.Infrastructure.Logging;
using System.Runtime.Versioning;

namespace FileTransferClient.Core.Exceptions
{
    public static class ExceptionHandler
    {
        /// <summary>
        /// Initializes global exception handling.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public static void ConfigureGlobalExceptionHandling()
        {
            System.Windows.Forms.Application.ThreadException += (sender, args) => HandleException(args.Exception);
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => HandleException(args.ExceptionObject as Exception);
        }

        /// <summary>
        /// Logs and displays error messages.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private static void HandleException(Exception ex)
        {
            if (ex == null) return;

            EventLogger.LogError("Unhandled Exception Occurred", ex);
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
