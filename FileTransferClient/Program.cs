using System;
using System.Windows.Forms;
using FileTransferClient.Core.DI;
using FileTransferClient.Presentation.Contracts;

namespace FileTransferClient
{
    static class Program
    {
        /// <summary>
        /// Main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            // Initialize Dependency Injection
            ServiceLocator.Configure();

            // Resolve the main form and start the application
            var mainView = ServiceLocator.Resolve<IMainView>();
            System.Windows.Forms.Application.Run((Form)mainView);
        }
    }
}
