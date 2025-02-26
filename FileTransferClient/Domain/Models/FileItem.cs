using System;

namespace FileTransferClient.Domain.Models
{
    /// <summary>
    /// Represents a file in the file transfer system.
    /// </summary>
    public class FileItem
    {
        public string Name { get; }
        public long Size { get; }
        public string FullPath { get; }

        public FileItem(string name, long size, string fullPath)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Size = size;
            FullPath = fullPath ?? throw new ArgumentNullException(nameof(fullPath));
        }
    }
}
