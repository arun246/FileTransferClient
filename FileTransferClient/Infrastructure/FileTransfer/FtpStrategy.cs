using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileTransferClient.Domain.Events;
using FileTransferClient.Domain.Models.Transfer;
using FileTransferClient.Domain.Models;
using FileTransferClient.Domain.Services;
using FluentFTP;

namespace FileTransferClient.Infrastructure.FileTransfer
{
    public class FtpStrategy : IFileTransferStrategy
    {
        public async Task TransferFileAsync(TransferJob job, Action<TransferProgressEvent> progressCallback)
        {
            using (var client = new FtpClient(job.ServerProfile.Host, job.ServerProfile.Username, job.ServerProfile.Password))
            {
                client.Connect();

                using (var fileStream = new FileStream(job.LocalPath, FileMode.Open, FileAccess.Read))
                {
                    long fileSize = fileStream.Length;
                    long bytesTransferred = 0;

                    await Task.Run(() => client.UploadStream(fileStream, job.DestinationPath, FtpRemoteExists.Overwrite, true, progress =>
                    {
                        bytesTransferred = (long)(fileSize * progress.Progress);
                        progressCallback?.Invoke(new TransferProgressEvent(new FileItem(Path.GetFileName(job.LocalPath), fileSize, job.LocalPath), (int)(progress.Progress * 100)));
                    }));
                }

                client.Disconnect();
            }
        }

        public async Task TransferFileAsync(FileItem file, CancellationToken cancellationToken, Action<int> progressCallback)
        {
            // Assuming FileItem does not have ServerProfile, we need to pass the server details separately
            // Adding server details as parameters to the method
            var serverProfile = new Domain.Models.Connection.ServerProfile("host", 21, "username", "password"); // Replace with actual values or parameters

            using (var client = new FtpClient(serverProfile.Host, serverProfile.Username, serverProfile.Password))
            {
                client.Connect();

                using (var fileStream = new FileStream(file.FullPath, FileMode.Open, FileAccess.Read))
                {
                    long fileSize = fileStream.Length;
                    long bytesTransferred = 0;

                    await Task.Run(() => client.UploadStream(fileStream, file.FullPath, FtpRemoteExists.Overwrite, true, progress =>
                    {
                        bytesTransferred = (long)(fileSize * progress.Progress);
                        progressCallback?.Invoke((int)(progress.Progress * 100));
                    }));
                }

                client.Disconnect();
            }
        }
    }
}
