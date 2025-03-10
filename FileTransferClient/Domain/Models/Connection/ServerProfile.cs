using FluentFTP.Client.BaseClient;
using FluentFTP;
using System.Net;
using System;
using Renci.SshNet; // Add this for SFTP

namespace FileTransferClient.Domain.Models.Connection
{
    public class ServerProfile
    {
        public int Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Stored securely using CredentialVault

        public ServerProfile(string host, int port, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(host)) throw new ArgumentException("Host is required");
            if (port <= 0) throw new ArgumentException("Port must be greater than zero");
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username is required");
            

            Host = host;
            Port = port;
            Username = username;
            Password = password;
        }

        public void ConnectToFtp()
        {
            using (var client = new FtpClient())
            {
                client.Host = Host;
                client.Port = Port;
                client.Credentials = new NetworkCredential(Username, Password);

                // Accept all certificates (not recommended for production)
                client.ValidateCertificate += OnValidateCertificate;

                client.Connect();

                if (client.IsConnected)
                {
                    System.Console.WriteLine("Connected to FTP server successfully.");
                    // Example: List directory contents
                    foreach (var item in client.GetListing())
                    {
                        System.Console.WriteLine(item.FullName);
                    }
                }
                else
                {
                    Console.WriteLine("Failed to connect to FTP server.");
                }
            }
        }

        public void ConnectToSftp()
        {
            using (var client = new SftpClient(Host, Port, Username, Password))
            {
                client.Connect();

                if (client.IsConnected)
                {
                    Console.WriteLine("Connected to SFTP server successfully.");
                    // Example: List directory contents
                    foreach (var item in client.ListDirectory("."))
                    {
                        Console.WriteLine(item.FullName);
                    }
                }
                else
                {
                    Console.WriteLine("Failed to connect to SFTP server.");
                }
            }
        }

        private void OnValidateCertificate(BaseFtpClient control, FtpSslValidationEventArgs e)
        {
            // Accept all certificates
            e.Accept = true;
        }
    }
}