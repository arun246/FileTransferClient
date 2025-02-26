using System;
using System.Collections.Generic;
using FileTransferClient.Domain.Models;

namespace FileTransferClient.Presentation.Contracts
{
    /// <summary>
    /// Interface defining the contract for the file browser view.
    /// </summary>
    public interface IFileBrowserView
    {
        /// <summary>
        /// Event triggered when a directory is selected.
        /// </summary>
        event EventHandler<string> DirectorySelected;

        /// <summary>
        /// Event triggered when a file is selected.
        /// </summary>
        event EventHandler<FileItem> FileSelected;

        /// <summary>
        /// Displays the list of files in the UI.
        /// </summary>
        void DisplayFiles(IEnumerable<FileItem> files);
    }
}
