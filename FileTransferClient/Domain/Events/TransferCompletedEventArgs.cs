using System;
using FileTransferClient.Domain.Models;

namespace FileTransferClient.Domain.Events
{
    public class TransferCompletedEventArgs : EventArgs
    {
        public FileItem File { get; }

        public TransferCompletedEventArgs(FileItem file)
        {
            File = file;
        }
    }
}
