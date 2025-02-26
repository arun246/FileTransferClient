using System;
using FileTransferClient.Domain.Models;

namespace FileTransferClient.Domain.Events
{
    /// <summary>
    /// Event data for tracking file transfer progress.
    /// </summary>
    public class TransferProgressEvent : EventArgs
    {
        public FileItem File { get; }
        public int ProgressPercentage { get; }

        public TransferProgressEvent(FileItem file, int progressPercentage)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
            ProgressPercentage = progressPercentage;
        }
    }
}
