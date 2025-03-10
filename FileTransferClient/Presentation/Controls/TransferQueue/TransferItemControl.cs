using System.Windows.Forms;
using FileTransferClient.Domain.Models.Transfer;
using FileTransferClient.Infrastructure.Logging;

namespace FileTransferClient.Presentation.Controls.TransferQueue
{
    public partial class TransferItemControl : UserControl
    {
        public TransferJob Job { get; }

        public TransferItemControl(TransferJob job)
        {
            InitializeComponent();
            Job = job;

            lblFileName.Text = job.SourcePath; // Change FileName to SourcePath
            progressBar.Value = job.Status == TransferStatus.InProgress ? 50 : 0; // Example progress value

            btnCancel.Click += (s, e) => OnCancelRequested();
        }

        public void UpdateProgress(int progress)
        {
            progressBar.Value = progress;
        }

        private void OnCancelRequested()
        {
            EventLogger.LogInfo($"Cancel requested for {Job.SourcePath}"); // Change FileName to SourcePath
            Job.MarkFailed(); // Change Job.Cancel() to Job.MarkFailed()
        }
    }
}
