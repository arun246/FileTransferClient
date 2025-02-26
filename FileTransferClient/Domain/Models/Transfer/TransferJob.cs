using FileTransferClient.Domain.Models.Connection;
using FileTransferClient.Domain.Models.Transfer;
using System;

public class TransferJob
{
    public Guid Id { get; } = Guid.NewGuid();
    public string SourcePath { get; }
    public string DestinationPath { get; }
    public TransferPriority Priority { get; }
    public TransferStatus Status { get; private set; }
    public ServerProfile ServerProfile { get; }
    public string LocalPath { get; set; } // Added this property

    public TransferJob(string sourcePath, string destinationPath, TransferPriority priority, ServerProfile serverProfile)
    {
        SourcePath = sourcePath ?? throw new ArgumentNullException(nameof(sourcePath));
        DestinationPath = destinationPath ?? throw new ArgumentNullException(nameof(destinationPath));
        Priority = priority;
        ServerProfile = serverProfile ?? throw new ArgumentNullException(nameof(serverProfile));
        Status = TransferStatus.Queued;
    }

    public void MarkInProgress() => Status = TransferStatus.InProgress;
    public void MarkCompleted() => Status = TransferStatus.Completed;
    public void MarkFailed() => Status = TransferStatus.Failed;
}
