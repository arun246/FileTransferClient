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
        event EventHandler ConnectClicked;
        event EventHandler OpenTransferQueueClicked;
        event EventHandler ViewProfilesClicked;
        event EventHandler<FileItem> EnqueueFileClicked;
        void ShowMessage(string message, string title, MessageBoxIcon icon);
        void Show();
        string ServerAddress { get; }
        int PortNumber { get; }
        string Username { get; }
        string Password { get; }
        bool IsFtp { get; }
        bool IsSftp { get; }
        string Encryption { get; } // Add this property
    }

}
