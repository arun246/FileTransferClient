using System;
using System.Collections.Generic;
using FileTransferClient.Domain.Models;

namespace FileTransferClient.Presentation.Contracts
{
    /// <summary>
    /// Interface defining the contract for the transfer queue view.
    /// </summary>
    public interface ITransferQueueView
    {
        /// <summary>
        /// Event triggered when a transfer is requested.
        /// </summary>
        event EventHandler<FileItem> TransferRequested;

        /// <summary>
        /// Event triggered when a transfer cancellation is requested.
        /// </summary>
        event EventHandler<FileItem> CancelTransferRequested;

        /// <summary>
        /// Updates the transfer queue in the UI.
        /// </summary>
        void UpdateTransferQueue(IEnumerable<FileItem> transferQueue);
    }
}
