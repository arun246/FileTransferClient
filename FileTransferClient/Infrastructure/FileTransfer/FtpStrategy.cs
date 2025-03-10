using FileTransferClient.Domain.Events;
using FileTransferClient.Domain.Models.Connection;
using FileTransferClient.Domain.Models;
using FileTransferClient.Domain.Services;
using FileTransferClient.Infrastructure.Logging;
using FluentFTP.Client.BaseClient;
using FluentFTP;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
using FluentFTP.Exceptions;

public class FtpStrategy : IFileTransferStrategy
{
    private readonly string _encryption;

    public FtpStrategy(string encryption)
    {
        _encryption = encryption;
    }

    public async Task TransferFileAsync(TransferJob job, Action<TransferProgressEvent> progressCallback)
    {
        try
        {
            using (var client = new FtpClient(job.ServerProfile.Host, job.ServerProfile.Username, job.ServerProfile.Password))
            {
                SetEncryptionMode(client);
                client.ValidateCertificate += OnValidateCertificate; // Validate certificate

                // Set timeout settings
                client.Config.ConnectTimeout = 30000; // 30 seconds
                client.Config.ReadTimeout = 30000; // 30 seconds
                client.Config.DataConnectionConnectTimeout = 30000; // 30 seconds
                client.Config.DataConnectionReadTimeout = 30000; // 30 seconds

                await Task.Run(() => client.Connect());

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
        catch (Exception ex)
        {
            EventLogger.LogError("Error during FTP file transfer", ex);
            throw;
        }
    }

    public async Task TransferFileAsync(FileItem file, CancellationToken cancellationToken, Action<int> progressCallback)
    {
        try
        {
            var serverProfile = new ServerProfile("host", 21, "username", "password"); // Replace with actual values or parameters

            using (var client = new FtpClient(serverProfile.Host, serverProfile.Username, serverProfile.Password))
            {
                SetEncryptionMode(client);
                client.ValidateCertificate += OnValidateCertificate; // Validate certificate

                // Set timeout settings
                client.Config.ConnectTimeout = 30000; // 30 seconds
                client.Config.ReadTimeout = 30000; // 30 seconds
                client.Config.DataConnectionConnectTimeout = 30000; // 30 seconds
                client.Config.DataConnectionReadTimeout = 30000; // 30 seconds

                await Task.Run(() => client.Connect());

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
        catch (Exception ex)
        {
            EventLogger.LogError("Error during FTP file transfer", ex);
            throw;
        }
    }

    public async Task TestConnectionAsync(ServerProfile profile)
    {
        try
        {
            using (var client = new FtpClient(profile.Host, profile.Username, profile.Password))
            {
                SetEncryptionMode(client);
                client.ValidateCertificate += OnValidateCertificate; // Validate certificate

                // Set timeout settings
                client.Config.ConnectTimeout = 30000; // 30 seconds
                client.Config.ReadTimeout = 30000; // 30 seconds
                client.Config.DataConnectionConnectTimeout = 30000; // 30 seconds
                client.Config.DataConnectionReadTimeout = 30000; // 30 seconds

                await Task.Run(() => client.Connect());
                if (!client.IsConnected)
                {
                    throw new Exception("FTP connection failed.");
                }
                client.Disconnect();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("FTP connection failed.", ex);
        }
    }

    public async Task<IEnumerable<FileItem>> ListDirectoryAsync(ServerProfile profile, string directoryPath)
    {
        var fileItems = new List<FileItem>();

        try
        {
            using (var client = new FtpClient(profile.Host, profile.Username, profile.Password))
            {
                SetEncryptionMode(client);
                client.ValidateCertificate += OnValidateCertificate;

                // Set timeout settings
                client.Config.ConnectTimeout = 30000; // 30 seconds
                client.Config.ReadTimeout = 30000; // 30 seconds
                client.Config.DataConnectionConnectTimeout = 30000; // 30 seconds
                client.Config.DataConnectionReadTimeout = 30000; // 30 seconds

                await Task.Run(() => client.Connect());

                var items = await Task.Run(() => client.GetListing(directoryPath));
                foreach (var item in items)
                {
                    if (item.Type == FtpObjectType.File)
                    {
                        fileItems.Add(new FileItem(item.Name, item.Size, item.FullName));
                    }
                }

                client.Disconnect();
            }
        }
        catch (Exception ex)
        {
            EventLogger.LogError("Error listing directory contents", ex);
            throw;
        }

        return fileItems;
    }

    public async Task CreateFileAsync(ServerProfile profile, string filePath)
    {
        try
        {
            if (!await VerifyFilePathAsync(profile, filePath))
            {
                throw new Exception("The specified file path is invalid or inaccessible on the FTP server.");
            }

            using (var client = new FtpClient(profile.Host, profile.Username, profile.Password))
            {
                SetEncryptionMode(client);
                client.ValidateCertificate += OnValidateCertificate;

                // Set timeout settings
                client.Config.ConnectTimeout = 30000; // 30 seconds
                client.Config.ReadTimeout = 30000; // 30 seconds
                client.Config.DataConnectionConnectTimeout = 30000; // 30 seconds
                client.Config.DataConnectionReadTimeout = 30000; // 30 seconds

                await Task.Run(() => client.Connect());

                using (var stream = new MemoryStream())
                {
                    await Task.Run(() => client.UploadStream(stream, filePath, FtpRemoteExists.NoCheck));
                }

                client.Disconnect();
            }
        }
        catch (TimeoutException ex)
        {
            EventLogger.LogError("Timeout error while creating file on FTP server", ex);
            throw new Exception("The operation timed out while creating the file on the FTP server. Please try again later.", ex);
        }
        catch (FtpException ex)
        {
            EventLogger.LogError("FTP error while creating file on FTP server", ex);
            throw new Exception("An FTP error occurred while creating the file on the FTP server. Please check the server settings and try again.", ex);
        }
        catch (IOException ex)
        {
            EventLogger.LogError("IO error while creating file on FTP server", ex);
            throw new Exception("An I/O error occurred while creating the file on the FTP server. Please check the file path and try again.", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            EventLogger.LogError("Unauthorized access error while creating file on FTP server", ex);
            throw new Exception("Unauthorized access while creating the file on the FTP server. Please check the credentials and try again.", ex);
        }
        catch (Exception ex)
        {
            EventLogger.LogError("Unexpected error while creating file on FTP server", ex);
            throw new Exception("An unexpected error occurred while creating the file on the FTP server. Please try again later.", ex);
        }
    }

    private async Task<bool> VerifyFilePathAsync(ServerProfile profile, string filePath)
    {
        try
        {
            using (var client = new FtpClient(profile.Host, profile.Username, profile.Password))
            {
                SetEncryptionMode(client);
                client.ValidateCertificate += OnValidateCertificate;

                // Set timeout settings
                client.Config.ConnectTimeout = 30000; // 30 seconds
                client.Config.ReadTimeout = 30000; // 30 seconds
                client.Config.DataConnectionConnectTimeout = 30000; // 30 seconds
                client.Config.DataConnectionReadTimeout = 30000; // 30 seconds

                await Task.Run(() => client.Connect());

                // Get the directory path from the file path
                string directoryPath = Path.GetDirectoryName(filePath).Replace("\\", "/");

                // List the directory contents
                var items = await Task.Run(() => client.GetListing(directoryPath));
                foreach (var item in items)
                {
                    if (item.Type == FtpObjectType.File && item.FullName == filePath)
                    {
                        // File already exists
                        return true;
                    }
                }

                client.Disconnect();
            }
        }
        catch (Exception ex)
        {
            EventLogger.LogError("Error verifying file path on FTP server", ex);
            throw new Exception("An error occurred while verifying the file path on the FTP server.", ex);
        }

        // File does not exist
        return false;
    }

    private void SetEncryptionMode(FtpClient client)
    {
        switch (_encryption)
        {
            case "Explicit":
                client.Config.EncryptionMode = FtpEncryptionMode.Explicit;
                break;
            case "Implicit":
                client.Config.EncryptionMode = FtpEncryptionMode.Implicit;
                break;
            default:
                client.Config.EncryptionMode = FtpEncryptionMode.None;
                break;
        }
    }

    private void OnValidateCertificate(BaseFtpClient control, FtpSslValidationEventArgs e)
    {
        // Accept all certificates (not recommended for production)
        e.Accept = true;
    }
}
