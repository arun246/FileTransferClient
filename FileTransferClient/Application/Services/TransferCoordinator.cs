using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using FileTransferClient.Application.Contracts;
using FileTransferClient.Domain.Models;
using FileTransferClient.Domain.Services;
using FileTransferClient.Domain.Events;
using System.Collections.Generic;

namespace FileTransferClient.Application.Services
{
    public class TransferCoordinator : ITransferQueue
    {
        private readonly IEnumerable<IFileTransferStrategy> _transferStrategies;
        private readonly ConcurrentQueue<FileItem> _transferQueue;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _isProcessing;

        public event EventHandler<TransferProgressEvent> TransferProgressUpdated;
        public event EventHandler<TransferCompletedEventArgs> TransferCompleted;

        public TransferCoordinator(IEnumerable<IFileTransferStrategy> transferStrategies)
        {
            _transferStrategies = transferStrategies ?? throw new ArgumentNullException(nameof(transferStrategies));
            _transferQueue = new ConcurrentQueue<FileItem>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Enqueues a file transfer and starts processing if not already running.
        /// </summary>
        public async Task EnqueueTransferAsync(FileItem file)
        {
            _transferQueue.Enqueue(file);
            if (!_isProcessing)
            {
                _isProcessing = true;
                await ProcessQueueAsync();
            }
        }

        /// <summary>
        /// Processes the transfer queue asynchronously.
        /// </summary>
        private async Task ProcessQueueAsync()
        {
            while (_transferQueue.TryDequeue(out var file))
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                    break;

                foreach (var strategy in _transferStrategies)
                {
                    await strategy.TransferFileAsync(file, _cancellationTokenSource.Token, progress =>
                    {
                        TransferProgressUpdated?.Invoke(this, new TransferProgressEvent(file, progress));
                    });
                }

                TransferCompleted?.Invoke(this, new TransferCompletedEventArgs(file));
            }

            _isProcessing = false;
        }

        /// <summary>
        /// Cancels an ongoing transfer.
        /// </summary>
        public void CancelTransfer(FileItem file)
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
