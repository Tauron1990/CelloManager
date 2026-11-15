using System;
using System.IO;
using System.Threading.Tasks;
using CelloManager.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Xunit;

namespace CelloManager.Avalonia.Tests;

public sealed class DataOperationManagerLoggingTests
{
    [Fact]
    public async Task LogsMigrationCommitAndFailureAndReportsWorkItemError()
    {
        string directory = Path.Combine(Path.GetTempPath(), "CelloManager.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string databasePath = Path.Combine(directory, "spools.db");
        RecordingLogger<DataOperationManager> logger = new();
        DataOperationManager manager = new(logger, databasePath);
        TaskCompletionSource successfulWork = new(TaskCreationOptions.RunContinuationsAsynchronously);
        InvalidOperationException expectedException = new("Test work item failure");
        TaskCompletionSource<Exception> reportedException = new(TaskCreationOptions.RunContinuationsAsynchronously);
        bool shutdownCalled = false;

        try
        {
            manager.RunItem(new DatabaseWorkitem(
                _ =>
                {
                    successfulWork.TrySetResult();
                    return ValueTask.CompletedTask;
                },
                exception => successfulWork.TrySetException(exception)));
            manager.RunItem(new DatabaseWorkitem(
                _ => ValueTask.FromException(expectedException),
                exception => reportedException.TrySetResult(exception)));

            await successfulWork.Task.WaitAsync(TimeSpan.FromSeconds(15));
            Assert.Same(expectedException, await reportedException.Task.WaitAsync(TimeSpan.FromSeconds(15)));
            shutdownCalled = true;
            await manager.Shutdown();

            Assert.Contains(logger.Events, item => item.EventId.Id == 1011 && item.Level == LogLevel.Information);
            Assert.Contains(logger.Events, item => item.EventId.Id == 1003 && item.Level == LogLevel.Debug);
            Assert.Contains(logger.Events, item => item.EventId.Id == 1004 && item.Level == LogLevel.Error && ReferenceEquals(item.Exception, expectedException));
            Assert.Contains(logger.Events, item => item.EventId.Id == 1006 && item.Level == LogLevel.Information);
            Assert.All(logger.Events, item => Assert.DoesNotContain(databasePath, item.Message + item.State, StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            if (!shutdownCalled)
            {
                await manager.Shutdown();
            }

            SqliteConnection.ClearAllPools();
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }
}
