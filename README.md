# FileTransferClient

FileTransferClient/
├─ Presentation/                      // Passive Views (Zero Business Logic)
│  ├─ Views/
│  │  ├─ MainForm.cs                 // Implements IMainView
│  │  ├─ ServerProfileDialog.cs
│  │  └─ TransferQueueWindow.cs
│  ├─ Controls/
│  │  ├─ FileBrowser/
│  │  │  ├─ FileBrowserControl.cs    // Implements IFileBrowserView
│  │  │  └─ FileItemTemplate.cs      // Custom ListView item template
│  │  └─ TransferQueue/
│  │     └─ TransferItemControl.cs
│  └─ Contracts/                     // View Interfaces
│     ├─ IMainView.cs
│     └─ IFileBrowserView.cs
│
├─ Application/                       // Presenters & Application Services
│  ├─ Presenters/
│  │  ├─ MainPresenter.cs            // Implements IAsyncDisposable
│  │  ├─ FileBrowserPresenter.cs
│  │  └─ TransferQueuePresenter.cs
│  ├─ Services/
│  │  ├─ TransferCoordinator.cs      // Orchestrates transfers
│  │  └─ ProfileManager.cs
│  ├─ Factories/
│  │  └─ TransferServiceFactory.cs   // Strategy Pattern
│  └─ Contracts/
│     ├─ ITransferQueue.cs
│     └─ IProfileManager.cs
│
├─ Domain/                           // Core Business Logic
│  ├─ Models/
│  │  ├─ Transfer/
│  │  │  ├─ TransferJob.cs           // State: Queued, InProgress, Completed
│  │  │  └─ TransferPriority.cs
│  │  └─ Connection/
│  │     └─ ServerProfile.cs         // Value Objects for credentials
│  ├─ Services/
│  │  └─ IFileTransferStrategy.cs    // Strategy Interface
│  └─ Events/
│     └─ TransferProgressEvent.cs
│
├─ Infrastructure/                   // Concrete Implementations
│  ├─ FileTransfer/
│  │  ├─ FtpStrategy.cs              // FluentFTP
│  │  └─ SftpStrategy.cs             // SSH.NET
│  ├─ Data/
│  │  ├─ Profiles/
│  │  │  ├─ ProfileRepository.cs     // EF Core
│  │  │  └─ ProfileConfiguration.cs  // Entity Configuration
│  │  └─ AppDbContext.cs
│  ├─ Security/
│  │  └─ CredentialVault.cs          // DPAPI Encryption
│  └─ Logging/
│     └─ EventLogger.cs              // Serilog Sink
│
├─ Core/                             // Cross-Cutting Concerns
│  ├─ DI/
│  │  └─ ServiceLocator.cs           // DI Container Setup
│  ├─ Threading/
│  │  └─ TransferSynchronizer.cs     // Async Coordination
│  └─ Exceptions/
│     └─ ExceptionHandler.cs         // Global Error Handling
│
└─ Program.cs                        // Composition Root
 
