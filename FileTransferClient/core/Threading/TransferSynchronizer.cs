using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using FileTransferClient.Domain.Models.Transfer;
using FileTransferClient.Domain.Models; // Corrected namespace
using FileTransferClient.Infrastructure.FileTransfer; // Add this to use FtpStrategy
using System.Windows.Forms;
using System; // Add this to use Control for UI updates

namespace FileTransferClient.Core.Threading
{
    public class TransferSynchronizer
    {
        private readonly BlockingCollection<TransferJob> _transferQueue;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ProgressBar _progressBar; // Add a reference to the progress bar
        private readonly string _encryption; // Add a reference to the encryption method

        public TransferSynchronizer(ProgressBar progressBar, string encryption)
        {
            _transferQueue = new BlockingCollection<TransferJob>(new ConcurrentQueue<TransferJob>());
            _cancellationTokenSource = new CancellationTokenSource();
            _progressBar = progressBar; // Initialize the progress bar reference
            _encryption = encryption; // Initialize the encryption method reference
        }

        /// <summary>
        /// Enqueues a new transfer job.
        /// </summary>
        public void EnqueueTransfer(TransferJob job)
        {
            _transferQueue.Add(job);
        }

        /// <summary>
        /// Processes the transfer queue asynchronously.
        /// </summary>
        public async Task ProcessTransfersAsync()
        {
            foreach (var job in _transferQueue.GetConsumingEnumerable(_cancellationTokenSource.Token))
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                    break;

                await ExecuteTransferJobAsync(job);
            }
        }

        /// <summary>
        /// Cancels all running transfers.
        /// </summary>
        public void CancelAllTransfers()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Executes the transfer job asynchronously.
        /// </summary>
        private async Task ExecuteTransferJobAsync(TransferJob job)
        {
            try
            {
                job.MarkInProgress();
                var ftpStrategy = new FtpStrategy(_encryption);
                await ftpStrategy.TransferFileAsync(job, progress =>
                {
                    // Update progress bar here
                    UpdateProgressBar(progress.ProgressPercentage);
                });
                job.MarkCompleted();
            }
            catch
            {
                job.MarkFailed();
            }
        }

        private void UpdateProgressBar(int progressPercentage)
        {
            if (_progressBar.InvokeRequired)
            {
                _progressBar.Invoke(new Action(() => _progressBar.Value = progressPercentage));
            }
            else
            {
                _progressBar.Value = progressPercentage;
            }
        }
    }
}


