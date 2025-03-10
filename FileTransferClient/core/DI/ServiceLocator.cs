﻿using FileTransferClient.Application.Contracts;
using FileTransferClient.Application.Presenters;
using FileTransferClient.Application.Services;
using FileTransferClient.Domain.Services;
using FileTransferClient.Infrastructure.Data.Profiles;
using FileTransferClient.Infrastructure.Data;
using FileTransferClient.Infrastructure.FileTransfer;
using FileTransferClient.Infrastructure.Logging;
using FileTransferClient.Presentation.Contracts;
using FileTransferClient.Presentation.Controls.FileBrowser;
using System;
using SimpleInjector;
using Microsoft.EntityFrameworkCore;
using FileTransferClient.Presentation.Views;
using System.IO;
using Newtonsoft.Json.Linq;

namespace FileTransferClient.Core.DI
{
    public static class ServiceLocator
    {
        private static readonly Container _container = new Container();

        /// <summary>
        /// Configures the Dependency Injection container.
        /// </summary>
        public static void Configure()
        {
            // Register Views (UI)
            _container.Register<FileTransferClient.Presentation.Contracts.IMainView, MainForm>(Lifestyle.Singleton);
            _container.Register<IFileBrowserView, FileBrowserControl>(Lifestyle.Singleton);

            // Register Presenters
            _container.Register<MainPresenter>(Lifestyle.Singleton);
            _container.Register<FileBrowserPresenter>(Lifestyle.Singleton);

            // Register Application Services
            _container.Register<ITransferQueue, TransferCoordinator>(Lifestyle.Singleton);
            _container.Register<IProfileManager, ProfileManager>(Lifestyle.Singleton);

            // Register Infrastructure (Persistence, Logging, Security)
            _container.Register<AppDbContext>(Lifestyle.Singleton);
            _container.Register<IProfileRepository, ProfileRepository>(Lifestyle.Singleton);
            _container.Register<ILogger, EventLoggerImplementation>(Lifestyle.Singleton);

            // Register DbContextOptions<AppDbContext>
            _container.Register(() =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer("data source=DESKTOP-CVVHELJ;initial catalog=master;Trusted_Connection=True;TrustServerCertificate=True;");
                return optionsBuilder.Options;
            }, Lifestyle.Singleton);

            // Read encryption setting from appsettings.json
            var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            var json = File.ReadAllText(configFilePath);
            var jsonObj = JObject.Parse(json);
            string encryption = jsonObj["EncryptionSettings"]["EncryptionType"].ToString();

            // Register File Transfer Strategies
            _container.Register<IFileTransferStrategy>(() => new FtpStrategy(encryption), Lifestyle.Singleton);
            _container.Collection.Register<IFileTransferStrategy>(new[] { typeof(SftpStrategy) });

            // Verify the container configuration
            _container.Verify();
        }

        /// <summary>
        /// Resolves a service from the DI container.
        /// </summary>
        public static T Resolve<T>() where T : class => _container.GetInstance<T>();
    }

    // Implementation of ILogger using EventLogger
    public class EventLoggerImplementation : ILogger
    {
        public void LogInfo(string message) => EventLogger.LogInfo(message);
        public void LogWarning(string message) => EventLogger.LogWarning(message);
        public void LogError(string message, Exception ex = null) => EventLogger.LogError(message, ex);
    }
}
