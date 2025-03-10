using System;
using System.Runtime.Versioning;
using FileTransferClient.Application.Contracts;

using System.Threading.Tasks;
using System.Windows.Forms;
using FileTransferClient.Presentation.Views;
using FileTransferClient.Domain.Models;
using FileTransferClient.Infrastructure.FileTransfer;
using FileTransferClient.Domain.Models.Transfer;
using FileTransferClient.Domain.Services;
using FileTransferClient.Domain.Models.Connection;
using FileTransferClient.Application.Factories;
using FileTransferClient.Presentation.Contracts;

namespace FileTransferClient.Application.Presenters
{
    public class MainPresenter : IDisposable
    {
        private readonly IMainView _view;
        private readonly IProfileManager _profileManager;
        private readonly ITransferQueue _transferQueue; // Add reference to other form view
        private bool _disposed = false;

        public MainPresenter(IMainView view, IProfileManager profileManager, ITransferQueue transferQueue)
        {
            _view = view;
            _profileManager = profileManager;
            _transferQueue = transferQueue;
            // Initialize other form view

            // Subscribe to view events
            if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            {
                _view.ConnectClicked += OnConnectClicked;
                _view.OpenTransferQueueClicked += OnOpenTransferQueueClicked;
                _view.EnqueueFileClicked += OnEnqueueFileClickedAsync;
                _view.ViewProfilesClicked += OnViewProfilesClicked; // Subscribe to new event
            }
        }

        /// <summary>
        /// Handles server connection logic.
        /// </summary>
        [SupportedOSPlatform("windows6.1")]
        private async void OnConnectClicked(object sender, EventArgs e)
        {
            try
            {
                var serverAddress = _view.ServerAddress;
                var portNumber = _view.PortNumber;
                var username = _view.Username;
                var password = _view.Password;
                var encryption = _view.Encryption; // Get the encryption method

                if (string.IsNullOrEmpty(serverAddress) || string.IsNullOrEmpty(username))
                {
                    _view.ShowMessage("Please provide all connection details.", "Warning", MessageBoxIcon.Warning);
                    return;
                }

                var profile = new ServerProfile(serverAddress, portNumber, username, password);
                IFileTransferStrategy transferStrategy;

                if (_view.IsFtp)
                {
                    transferStrategy = TransferServiceFactory.CreateStrategy("ftp", encryption);
                }
                else if (_view.IsSftp)
                {
                    transferStrategy = TransferServiceFactory.CreateStrategy("sftp");
                }
                else
                {
                    _view.ShowMessage("Please select either FTP or SFTP.", "Warning", MessageBoxIcon.Warning);
                    return;
                }

                // Test connection
                await transferStrategy.TestConnectionAsync(profile);

                _view.ShowMessage("Server connection successful.", "Info", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Error: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Opens the transfer queue window.
        /// </summary>
        [SupportedOSPlatform("windows6.1")]
        private void OnOpenTransferQueueClicked(object sender, EventArgs e)
        {
            var transferQueueWindow = new TransferQueueWindow();
            transferQueueWindow.Show();
            _view.ShowMessage("Transfer Queue opened.", "Info", MessageBoxIcon.Information);
        }

        /// <summary>
        /// Handles file enqueue logic.
        /// </summary>
        [SupportedOSPlatform("windows6.1")]
        private async void OnEnqueueFileClickedAsync(object sender, FileItem e)
        {
            // Handle file enqueue logic here
            try
            {
                await _transferQueue.EnqueueTransferAsync(e);
                _view.ShowMessage($"File {e.Name} enqueued.", "Info", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Error enqueuing file: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles viewing profiles logic.
        /// </summary>
        [SupportedOSPlatform("windows6.1")]
        private async void OnViewProfilesClicked(object sender, EventArgs e)
        {
            try
            {
                var profiles = await _profileManager.GetAllProfilesAsync();
                var profileListForm = new ProfileListForm(profiles);
                profileListForm.Show();
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Error: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Dispose resources if needed.
        /// </summary>
        public void Dispose()
        {
            if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            {
                Dispose(true);
            }
            GC.SuppressFinalize(this);
        }

        [SupportedOSPlatform("windows6.1")]
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Unsubscribe from view events
                    _view.ConnectClicked -= OnConnectClicked;
                    _view.OpenTransferQueueClicked -= OnOpenTransferQueueClicked;
                    _view.EnqueueFileClicked -= OnEnqueueFileClickedAsync;
                    _view.ViewProfilesClicked -= OnViewProfilesClicked; // Unsubscribe from new event
                }

                _disposed = true;
            }
        }

        public async Task DisposeAsync()
        {
            Dispose();
            await Task.FromResult(0);
        }
    }
    // Add the missing property to the IMainView interface
    
}


