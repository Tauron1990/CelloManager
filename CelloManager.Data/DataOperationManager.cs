using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CelloManager.Data;

public sealed class DataOperationManager
{
    private readonly ILogger<DataOperationManager> _logger;
    private readonly Task _workerThread;

    private readonly BlockingCollection<DatabaseWorkitem> _workitems = new();
    private readonly string? _databasePath;
    private SpoolDataBase? _dataBase;

    private bool _needMigration = true;

    [RequiresUnreferencedCode("Calls CelloManager.Data.DataOperationManager.ProcessItems()")]
    public DataOperationManager(ILogger<DataOperationManager> logger) : this(logger, null)
    {
    }

    [RequiresUnreferencedCode("Calls CelloManager.Data.DataOperationManager.ProcessItems()")]
    internal DataOperationManager(ILogger<DataOperationManager> logger, string? databasePath)
    {
        _logger = logger;
        _databasePath = databasePath;
        _workerThread = Task.Run(ProcessItems);
    }

    public static string DatabaseDic
    {
        get
        {
            string databaseDic = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Tauron", "CelloManager");
            if (!Directory.Exists(databaseDic))
            {
                Directory.CreateDirectory(databaseDic);
            }

            return databaseDic;
        }
    }

    public async ValueTask Shutdown()
    {
        _workitems.CompleteAdding();
        await _workerThread.ConfigureAwait(false);
    }

    public void RunItem(DatabaseWorkitem item)
    {
        if (_workitems.IsAddingCompleted)
        {
            _logger.LogDebug(new EventId(1001, "WorkItemRejected"), "Database work item rejected because the worker is shutting down");
            return;
        }

        _workitems.Add(item);
        _logger.LogDebug(new EventId(1002, "WorkItemQueued"), "Database work item queued; {PendingWorkItemCount} items pending", _workitems.Count);
    }

    public Task<IEnumerable<TSelector>> Query<TData, TSelector>(
        Func<SpoolDataBase, DbSet<TData>> setSelector,
        Func<IQueryable<TData>, IQueryable<TSelector>> converter)
        where TData : class
    {
        var source = new TaskCompletionSource<IEnumerable<TSelector>>(TaskCreationOptions.RunContinuationsAsynchronously);

        DatabaseWorkitem workItem = new(
            async db =>
            {
                DbSet<TData> set = setSelector(db);
                IQueryable<TSelector> selector = converter(set);
                List<TSelector> result = await selector.ToListAsync().ConfigureAwait(false);
                source.SetResult(result);
            },
            source.SetException);

        RunItem(workItem);

        return source.Task;
    }

    [RequiresUnreferencedCode("Calls CelloManager.Data.DataOperationManager.CreateDatabase()")]
    private async Task ProcessItems()
    {
        _logger.LogInformation(new EventId(1000, "WorkerStarted"), "Database worker started");
        try
        {
            await Task.Yield();

            foreach (DatabaseWorkitem workitem in _workitems.GetConsumingEnumerable())
            {
                try
                {
                    _dataBase ??= await CreateDatabase().ConfigureAwait(false);

                    await workitem.Worker(_dataBase).ConfigureAwait(false);

                    await _dataBase.SaveChangesAsync().ConfigureAwait(false);
                    _logger.LogDebug(new EventId(1003, "WorkItemCommitted"), "Database work item committed");
                }
                catch (Exception e)
                {
                    _logger.LogError(new EventId(1004, "WorkItemFailed"), e, "Database work item failed");
                    try
                    {
                        workitem.ErrorReporter(e);
                    }
                    catch (Exception callbackException)
                    {
                        _logger.LogError(new EventId(1005, "ErrorReporterFailed"), callbackException, "Database work item error callback failed");
                    }
                }

                if (_dataBase is null || _workitems.Count != 0)
                {
                    continue;
                }

                await DisposeDatabase().ConfigureAwait(false);
            }
        }
        finally
        {
            try
            {
                await DisposeDatabase().ConfigureAwait(false);
            }
            finally
            {
                _logger.LogInformation(new EventId(1006, "WorkerStopped"), "Database worker stopped");
            }
        }
    }

    [RequiresUnreferencedCode("Calls CelloManager.Data.SpoolDataBase.SpoolDataBase(DbContextOptions<SpoolDataBase>)")]
    private async ValueTask<SpoolDataBase> CreateDatabase()
    {
        SpoolDataBase dataBase = new(
            new DbContextOptionsBuilder<SpoolDataBase>()
               .UseSqlite(new SqliteConnectionStringBuilder
                          {
                              Pooling = true,
                              DataSource = _databasePath ?? Path.Combine(DatabaseDic, "spools.db"),
                          }.ConnectionString)
               .Options);

        try
        {
            if (_needMigration)
            {
                _logger.LogInformation(new EventId(1010, "MigrationStarted"), "Database migration started");
                await dataBase.Database.MigrateAsync().ConfigureAwait(false);
                _needMigration = false;
                _logger.LogInformation(new EventId(1011, "MigrationCompleted"), "Database migration completed");
            }

            await dataBase.Database.ExecuteSqlAsync($"VACUUM;").ConfigureAwait(false);
            _logger.LogDebug(new EventId(1012, "DatabaseReady"), "Database initialized and ready");

            return dataBase;
        }
        catch
        {
            await dataBase.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    private async ValueTask DisposeDatabase()
    {
        if (_dataBase is not { } dataBase)
        {
            return;
        }

        _dataBase = null;
        try
        {
            await dataBase.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.LogError(new EventId(1013, "DatabaseDisposeFailed"), exception, "Database context disposal failed");
            throw;
        }
    }
}