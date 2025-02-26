using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FileTransferClient.Presentation.Contracts;
using FileTransferClient.Domain.Models;

namespace FileTransferClient.Presentation.Views
{
    public partial class TransferQueueWindow : Form, ITransferQueueView
    {
        public event EventHandler<FileItem> TransferRequested;
        public event EventHandler<FileItem> CancelTransferRequested;

        private ListView transferListView;
        private Button btnCancel;

        public TransferQueueWindow()
        {
            InitializeComponent();
            SetupUI();
        }

        /// <summary>
        /// Initializes UI components.
        /// </summary>
        private void SetupUI()
        {
            this.Text = "Transfer Queue";
            this.Width = 500;
            this.Height = 400;

            transferListView = new ListView
            {
                Dock = DockStyle.Top,
                View = View.Details,
                FullRowSelect = true,
                Height = 300
            };
            transferListView.Columns.Add("File Name", 300);
            this.Controls.Add(transferListView);

            btnCancel = new Button
            {
                Text = "Cancel Transfer",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            btnCancel.Click += (s, e) =>
            {
                if (transferListView.SelectedItems.Count > 0)
                {
                    var file = (FileItem)transferListView.SelectedItems[0].Tag;
                    CancelTransferRequested?.Invoke(this, file);
                }
            };
            this.Controls.Add(btnCancel);
        }

        /// <summary>
        /// Updates the transfer queue display.
        /// </summary>
        public void UpdateTransferQueue(IEnumerable<FileItem> files)
        {
            transferListView.Items.Clear();
            foreach (var file in files)
            {
                var item = new ListViewItem(file.Name) { Tag = file };
                transferListView.Items.Add(item);
            }
        }

        private void TransferQueueWindow_Load(object sender, EventArgs e)
        {

        }
    }
}

