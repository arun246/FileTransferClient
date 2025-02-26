using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using FileTransferClient.Domain.Models.Transfer;
using FileTransferClient.Domain.Models; // Corrected namespace

namespace FileTransferClient.Core.Threading
{
    public class TransferSynchronizer
    {
        private readonly BlockingCollection<TransferJob> _transferQueue;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public TransferSynchronizer()
        {
            _transferQueue = new BlockingCollection<TransferJob>(new ConcurrentQueue<TransferJob>());
            _cancellationTokenSource = new CancellationTokenSource();
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
                // Simulate the transfer operation
                await Task.Delay(1000); // Replace with actual transfer logic
                job.MarkCompleted();
            }
            catch
            {
                job.MarkFailed();
            }
        }
    }
}
