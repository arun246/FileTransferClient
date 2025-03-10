using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileTransferClient.Domain.Models;
using FileTransferClient.Domain.Services;
using Renci.SshNet;
using FileTransferClient.Infrastructure.Logging;
using FileTransferClient.Domain.Events;
using FileTransferClient.Domain.Models.Connection;

namespace FileTransferClient.Infrastructure.FileTransfer
{
    public class SftpStrategy : IFileTransferStrategy
    {
        public async Task TransferFileAsync(FileItem file, CancellationToken cancellationToken, Action<int> progressCallback)
        {
            try
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
            catch (Exception ex)
            {
                EventLogger.LogError("Error during SFTP file transfer", ex);
                throw;
            }
        }

        public async Task TransferFileAsync(TransferJob job, Action<TransferProgressEvent> progressCallback)
        {
            try
            {
                using var sftpClient = new SftpClient(job.ServerProfile.Host, job.ServerProfile.Port, job.ServerProfile.Username, job.ServerProfile.Password);
                sftpClient.Connect();

                using var fileStream = new FileStream(job.LocalPath, FileMode.Open, FileAccess.Read);
                long totalBytes = fileStream.Length;
                long uploadedBytes = 0;

                await Task.Run(() =>
                {
                    sftpClient.UploadFile(fileStream, job.DestinationPath, uploaded =>
                    {
                        uploadedBytes += (long)uploaded;
                        int progress = (int)((uploadedBytes / (double)totalBytes) * 100);
                        progressCallback?.Invoke(new TransferProgressEvent(new FileItem(Path.GetFileName(job.LocalPath), totalBytes, job.LocalPath), progress));
                    });

                    sftpClient.Disconnect();
                });
            }
            catch (Exception ex)
            {
                EventLogger.LogError("Error during SFTP file transfer", ex);
                throw;
            }
        }

        public async Task TestConnectionAsync(ServerProfile profile)
        {
            try
            {
                using (var sftpClient = new SftpClient(profile.Host, profile.Port, profile.Username, profile.Password))
                {
                    await Task.Run(() => sftpClient.Connect());
                    if (!sftpClient.IsConnected)
                    {
                        throw new Exception("SFTP connection failed.");
                    }
                    sftpClient.Disconnect();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SFTP connection failed.", ex);
            }
        }
    }
}


