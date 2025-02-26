using System;
using System.Threading;
using System.Threading.Tasks;
using FileTransferClient.Domain.Models;

namespace FileTransferClient.Domain.Services
{
    /// <summary>
    /// Defines a strategy for transferring files.
    /// </summary>
    public interface IFileTransferStrategy
    {
        /// <summary>
        /// Transfers a file asynchronously using the defined protocol.
        /// </summary>
        Task TransferFileAsync(FileItem file, CancellationToken cancellationToken, Action<int> progressCallback);
    }
}
