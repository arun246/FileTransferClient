using System;
using System.Collections.Generic;
using FileTransferClient.Domain.Models;
using FileTransferClient.Presentation.Contracts;

namespace FileTransferClient.Application.Presenters
{
    public class TransferQueuePresenter
    {
        private readonly ITransferQueueView _view;
        private readonly List<FileItem> _transferQueue;

        public TransferQueuePresenter(ITransferQueueView view)
        {
            _view = view;
            _transferQueue = new List<FileItem>();

            // Subscribe to UI events
            _view.TransferRequested += OnTransferRequested;
            _view.CancelTransferRequested += OnCancelTransferRequested;
        }

        /// <summary>
        /// Adds a file to the transfer queue.
        /// </summary>
        private void OnTransferRequested(object sender, FileItem file)
        {
            if (!_transferQueue.Contains(file))
            {
                _transferQueue.Add(file);
                _view.UpdateTransferQueue(_transferQueue);
            }
        }

        /// <summary>
        /// Removes a file from the transfer queue.
        /// </summary>
        private void OnCancelTransferRequested(object sender, FileItem file)
        {
            _transferQueue.Remove(file);
            _view.UpdateTransferQueue(_transferQueue);
        }
    }
}
