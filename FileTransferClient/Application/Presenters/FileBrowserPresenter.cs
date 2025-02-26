using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileTransferClient.Domain.Models;
using FileTransferClient.Presentation.Contracts;

namespace FileTransferClient.Application.Presenters
{
    public class FileBrowserPresenter
    {
        private readonly IFileBrowserView _view;

        public FileBrowserPresenter(IFileBrowserView view)
        {
            _view = view;

            // Subscribe to view events
            _view.DirectorySelected += OnDirectorySelected;
        }

        /// <summary>
        /// Handles directory selection and loads files.
        /// </summary>
        private async void OnDirectorySelected(object sender, string path)
        {
            var files = await Task.Run(() => GetFilesFromDirectory(path));
            _view.DisplayFiles(files);
        }

        /// <summary>
        /// Retrieves files from a directory.
        /// </summary>
        private IEnumerable<FileItem> GetFilesFromDirectory(string path)
        {
            var files = new List<FileItem>();
            try
            {
                foreach (var filePath in Directory.GetFiles(path))
                {
                    var fileInfo = new FileInfo(filePath);
                    files.Add(new FileItem(fileInfo.Name, fileInfo.Length, filePath));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading files: {ex.Message}");
            }
            return files;
        }
    }
}
