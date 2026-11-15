using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CelloManager.Core.Data;
using CelloManager.Core.Logic;
using CelloManager.Data;
using Microsoft.Data.Sqlite;
using System.Reactive.Linq;
using Xunit;

namespace CelloManager.Avalonia.Tests;

public sealed class WorkflowLoggingTests
{
    [Fact]
    public async Task InventoryAndOrderLogsDescribeOutcomesWithoutSpoolDetails()
    {
        string directory = Path.Combine(Path.GetTempPath(), "CelloManager.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string databasePath = Path.Combine(directory, "spools.db");
        RecordingLogger<DataOperationManager> databaseLogger = new();
        RecordingLogger<SpoolRepository> repositoryLogger = new();
        RecordingLogger<SpoolManager> spoolLogger = new();
        RecordingLogger<OrderManager> orderLogger = new();
        DataOperationManager databaseManager = new(databaseLogger, databasePath);
        ErrorDispatcher errors = new();
        SpoolRepository repository = new(errors, databaseManager, repositoryLogger);
        bool shutdownCalled = false;

        try
        {
            await repository.Init();
            SpoolManager spoolManager = new(repository, spoolLogger);
            spoolManager.CreateSpool("private-spool-name", "private-category", 2, 4);
            Assert.True(repository.LookUp("private-spool-name", "private-category").HasValue);

            MemoryStorageFile exportProxy = MemoryStorageFile.Create(out IStorageFile exportFile);
            Assert.Null(await spoolManager.ExportToJson(exportFile));

            MemoryStorageFile importProxy = MemoryStorageFile.Create(out IStorageFile importFile);
            importProxy.Contents = exportProxy.WrittenContents ?? throw new InvalidOperationException("Export did not write a document");
            Assert.Null(await spoolManager.ImportFromJson(importFile));

            MemoryStorageFile failedExportProxy = MemoryStorageFile.Create(out IStorageFile failedExportFile);
            failedExportProxy.FailWrite = true;
            Assert.IsType<IOException>(await spoolManager.ExportToJson(failedExportFile));

            MemoryStorageFile failedImportProxy = MemoryStorageFile.Create(out IStorageFile failedImportFile);
            failedImportProxy.FailRead = true;
            Assert.IsType<IOException>(await spoolManager.ImportFromJson(failedImportFile));

            using OrderManager orderManager = new(repository, orderLogger);
            await WaitForCanOrder(orderManager, true);
            Assert.True(orderManager.PlaceOrder());
            await WaitForCanOrder(orderManager, false);
            Assert.False(orderManager.PlaceOrder());

            shutdownCalled = true;
            await databaseManager.Shutdown();

            Assert.Contains(spoolLogger.Events, item => item.EventId.Id == 2000 && item.Level == Microsoft.Extensions.Logging.LogLevel.Information);
            Assert.Contains(spoolLogger.Events, item => item.EventId.Id == 2010 && item.Level == Microsoft.Extensions.Logging.LogLevel.Information);
            Assert.Contains(spoolLogger.Events, item => item.EventId.Id == 2012 && item.Level == Microsoft.Extensions.Logging.LogLevel.Information);
            Assert.Contains(spoolLogger.Events, item => item.EventId.Id == 2011 && item.Level == Microsoft.Extensions.Logging.LogLevel.Error);
            Assert.Contains(spoolLogger.Events, item => item.EventId.Id == 2013 && item.Level == Microsoft.Extensions.Logging.LogLevel.Error);
            Assert.Contains(orderLogger.Events, item => item.EventId.Id == 3000 && item.Level == Microsoft.Extensions.Logging.LogLevel.Information);
            Assert.Contains(orderLogger.Events, item => item.EventId.Id == 3001 && item.Level == Microsoft.Extensions.Logging.LogLevel.Debug);
            string logText = string.Join(Environment.NewLine, spoolLogger.Events.Concat(orderLogger.Events).Select(item => item.Message + item.State));
            Assert.DoesNotContain("private-spool-name", logText, StringComparison.Ordinal);
            Assert.DoesNotContain("private-category", logText, StringComparison.Ordinal);
        }
        finally
        {
            if (!shutdownCalled)
            {
                await databaseManager.Shutdown();
            }

            repository.Dispose();
            errors.Dispose();
            SqliteConnection.ClearAllPools();
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }

    public class MemoryStorageFile : DispatchProxy
    {
        private MemoryStream? _writtenStream;

        public byte[] Contents { get; set; } = [];

        public bool FailRead { get; set; }

        public bool FailWrite { get; set; }

        public byte[]? WrittenContents => _writtenStream?.ToArray();

        public static MemoryStorageFile Create(out IStorageFile file)
        {
            file = DispatchProxy.Create<IStorageFile, MemoryStorageFile>();
            return (MemoryStorageFile)file;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            return targetMethod?.Name switch
            {
                "OpenReadAsync" => FailRead
                                         ? Task.FromException<Stream>(new IOException("Test import failure"))
                                         : Task.FromResult<Stream>(new MemoryStream(Contents, writable: false)),
                "OpenWriteAsync" => OpenWrite(),
                _ => throw new NotSupportedException($"Unsupported storage operation {targetMethod?.Name}"),
            };
        }

        private Task<Stream> OpenWrite()
        {
            if (FailWrite)
            {
                return Task.FromException<Stream>(new IOException("Test export failure"));
            }

            _writtenStream = new MemoryStream();
            return Task.FromResult<Stream>(_writtenStream);
        }
    }

    private static async Task WaitForCanOrder(OrderManager orderManager, bool expected)
    {
        TaskCompletionSource<bool> canOrder = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using IDisposable subscription = orderManager.CanOrder
                                                  .Where(value => value == expected)
                                                  .Take(1)
                                                  .Subscribe(value => canOrder.TrySetResult(value));
        await canOrder.Task.WaitAsync(TimeSpan.FromSeconds(10));
    }
}
