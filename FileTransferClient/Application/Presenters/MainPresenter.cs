using System;
using System.Runtime.Versioning;
using FileTransferClient.Application.Contracts;
using FileTransferClient.Presentation.Contracts;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileTransferClient.Presentation.Views;
using FileTransferClient.Domain.Models;

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
                // Subscribe to other form event
                _view.EnqueueFileClicked += OnEnqueueFileClicked;
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
                var profiles = await _profileManager.GetAllProfilesAsync();
                if (profiles.Count == 0)
                {
                    _view.ShowMessage("No server profiles found. Please add one.", "Warning", MessageBoxIcon.Warning);
                    return;
                }

                // TODO: Show server selection dialog
                _view.ShowMessage("Server connection initiated.", "Info", MessageBoxIcon.Information);
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
        private void OnEnqueueFileClicked(object sender, FileItem e)
        {
            // Handle file enqueue logic here
            _view.ShowMessage($"File {e.Name} enqueued.", "Info", MessageBoxIcon.Information);
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
                    // Unsubscribe from other form event
                    _view.EnqueueFileClicked -= OnEnqueueFileClicked;
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
}
