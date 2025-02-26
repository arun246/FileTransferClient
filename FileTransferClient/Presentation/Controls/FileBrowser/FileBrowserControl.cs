using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FileTransferClient.Domain.Models;
using FileTransferClient.Presentation.Contracts;

namespace FileTransferClient.Presentation.Controls.FileBrowser
{
    public partial class FileBrowserControl : UserControl, IFileBrowserView
    {
        public event EventHandler<string> DirectorySelected;
        public event EventHandler<FileItem> FileSelected;

        public FileBrowserControl()
        {
            InitializeComponent();
        }

        private void FileBrowserControl_Load(object sender, EventArgs e)
        {
            // Add your load logic here
        }

        public void DisplayFiles(IEnumerable<FileItem> files)
        {
            // Implement the logic to display files in the UI
        }

        protected virtual void OnDirectorySelected(string directory)
        {
            DirectorySelected?.Invoke(this, directory);
        }

        protected virtual void OnFileSelected(FileItem file)
        {
            FileSelected?.Invoke(this, file);
        }
    }
}
