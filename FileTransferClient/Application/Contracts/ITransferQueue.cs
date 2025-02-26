using System;
using System.Threading.Tasks;
using FileTransferClient.Domain.Events;
using FileTransferClient.Domain.Models;

namespace FileTransferClient.Application.Contracts
{
    /// <summary>
    /// Defines the contract for managing file transfer queue operations.
    /// </summary>
    public interface ITransferQueue
    {
        /// <summary>
        /// Adds a file to the transfer queue.
        /// </summary>
        Task EnqueueTransferAsync(FileItem file);

        /// <summary>
        /// Cancels an ongoing file transfer.
        /// </summary>
        void CancelTransfer(FileItem file);

        /// <summary>
        /// Event triggered when a transfer progresses.
        /// </summary>
        event EventHandler<TransferProgressEvent> TransferProgressUpdated;

        /// <summary>
        /// Event triggered when a transfer completes.
        /// </summary>
        event EventHandler<TransferCompletedEventArgs> TransferCompleted;
    }
}
