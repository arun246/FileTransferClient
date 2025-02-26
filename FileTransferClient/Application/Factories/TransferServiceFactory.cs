using System;
using FileTransferClient.Domain.Services;
using FileTransferClient.Infrastructure.FileTransfer;

namespace FileTransferClient.Application.Factories
{
    /// <summary>
    /// Factory class for creating appropriate file transfer strategy.
    /// </summary>
    public static class TransferServiceFactory
    {
        /// <summary>
        /// Creates a file transfer strategy based on protocol type.
        /// </summary>
        public static IFileTransferStrategy CreateStrategy(string protocolType)
        {
            switch (protocolType.ToLower())
            {
                case "ftp":
                    return new FtpStrategy();
                case "sftp":
                    return new SftpStrategy();
                default:
                    throw new ArgumentException("Unsupported protocol type", nameof(protocolType));
            }
        }
    }
}
