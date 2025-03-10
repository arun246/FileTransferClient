using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using FileTransferClient.Domain.Models;
using FileTransferClient.Presentation.Contracts;

namespace FileTransferClient.Presentation.Controls.FileBrowser
{
    public partial class FileBrowserControl : UserControl, IFileBrowserView
    {
        public event EventHandler<string> DirectorySelected;
        public event EventHandler<FileItem> FileSelected;

        private ListView listViewFiles;

        public FileBrowserControl()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            listViewFiles = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true
            };
            listViewFiles.Columns.Add("Name", 200);
            listViewFiles.Columns.Add("Size", 100);
            listViewFiles.ItemActivate += ListViewFiles_ItemActivate;
            this.Controls.Add(listViewFiles);
        }

        private void ListViewFiles_ItemActivate(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count > 0)
            {
                var selectedItem = listViewFiles.SelectedItems[0];
                var fileItem = (FileItem)selectedItem.Tag;
                OnFileSelected(fileItem);
            }
        }

        public void DisplayFiles(IEnumerable<FileItem> files)
        {
            listViewFiles.Items.Clear();
            foreach (var file in files)
            {
                var listViewItem = new ListViewItem(file.Name);
                listViewItem.SubItems.Add(file.Size.ToString());
                listViewItem.Tag = file;
                listViewFiles.Items.Add(listViewItem);
            }
        }

        private void FileBrowserControl_Load(object sender, EventArgs e)
        {
            // Set the initial directory (e.g., the user's documents folder)
            string initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            LoadDirectory(initialDirectory);
        }

        private void LoadDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                var files = new List<FileItem>();
                foreach (var filePath in Directory.GetFiles(path))
                {
                    var fileInfo = new FileInfo(filePath);
                    files.Add(new FileItem(fileInfo.Name, fileInfo.Length, fileInfo.FullName));
                }
                DisplayFiles(files);
                listViewFiles.Tag = path; // Set the current directory path
                OnDirectorySelected(path);
            }
            else
            {
                MessageBox.Show($"The directory '{path}' does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string GetCurrentDirectoryPath()
        {
            return listViewFiles.Tag?.ToString() ?? string.Empty;
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
