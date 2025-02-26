using System;
using System.Windows.Forms;
using FileTransferClient.Domain.Models;

namespace FileTransferClient.Presentation.Contracts
{
    /// <summary>
    /// Interface defining the contract for the main application view.
    /// </summary>
    public interface IMainView
    {
        /// <summary>
        /// Event triggered when the user attempts to connect to a server.
        /// </summary>
        event EventHandler ConnectClicked;

        /// <summary>
        /// Event triggered when the user wants to open transfer queue.
        /// </summary>
        event EventHandler OpenTransferQueueClicked;

        /// <summary>
        /// Event triggered when the user wants to open the other form.
        /// </summary>
        event EventHandler OpenOtherFormClicked; // Add this line

        /// <summary>
        /// Displays a message to the user.
        /// </summary>
        void ShowMessage(string message, string title, MessageBoxIcon icon);

        /// <summary>
        /// Event triggered when the user wants to enqueue a file for transfer.
        /// </summary>
        event EventHandler<FileItem> EnqueueFileClicked;

        /// <summary>
        /// Displays the view.
        /// </summary>
        void Show();
    }
}
