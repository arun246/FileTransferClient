using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileTransferClient.Domain.Models;
using FileTransferClient.Domain.Services;
using Renci.SshNet; // Add this using directive
using Renci.SshNet.Sftp;

namespace FileTransferClient.Infrastructure.FileTransfer
{
    public class SftpStrategy : IFileTransferStrategy
    {
        public async Task TransferFileAsync(FileItem file, CancellationToken cancellationToken, Action<int> progressCallback)
        {
            using var sftpClient = new SftpClient("sftp.example.com", "username", "password");
            sftpClient.Connect();

            using var fileStream = new FileStream(file.FullPath, FileMode.Open, FileAccess.Read);
            long totalBytes = fileStream.Length;
            long uploadedBytes = 0;

            await Task.Run(() =>
            {
                sftpClient.UploadFile(fileStream, "/remote/path/" + file.Name, uploaded =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        sftpClient.Disconnect();
                        throw new OperationCanceledException("Transfer canceled.");
                    }

                    uploadedBytes += (long)uploaded;
                    int progress = (int)((uploadedBytes / (double)totalBytes) * 100);
                    progressCallback?.Invoke(progress);
                });

                sftpClient.Disconnect();
            }, cancellationToken);
        }
    }
}
