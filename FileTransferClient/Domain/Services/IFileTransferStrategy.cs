using System;
using System.Threading;
using System.Threading.Tasks;
using FileTransferClient.Domain.Models;
using FileTransferClient.Domain.Models.Connection;

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

        /// <summary>
        /// Tests the connection to the server.
        /// </summary>
        Task TestConnectionAsync(ServerProfile profile);
    }
}
