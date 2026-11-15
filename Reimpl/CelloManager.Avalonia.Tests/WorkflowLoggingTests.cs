using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

            using OrderManager orderManager = new(repository, orderLogger);
            await WaitForCanOrder(orderManager, true);
            Assert.True(orderManager.PlaceOrder());
            await WaitForCanOrder(orderManager, false);
            Assert.False(orderManager.PlaceOrder());

            shutdownCalled = true;
            await databaseManager.Shutdown();

            Assert.Contains(spoolLogger.Events, item => item.EventId.Id == 2000 && item.Level == Microsoft.Extensions.Logging.LogLevel.Information);
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
