using System.Collections.Concurrent;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CelloManager.Data;

public sealed class DataOperationManager
{
    public static readonly DataOperationManager Manager = new();
    private readonly Task _workerThread;

    private readonly BlockingCollection<DatabaseWorkitem> _workitems = new();
    private SpoolDataBase? _dataBase;

    private bool _needMigration = true;

    private DataOperationManager() => _workerThread = Task.Run(ProcessItems);

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
            return;
        }

        _workitems.Add(item);
    }

    public Task<List<TSelector>> Query<TData, TSelector>(
        Func<SpoolDataBase, DbSet<TData>> setSelector,
        Func<IQueryable<TData>, IQueryable<TSelector>> converter)
        where TData : class
    {
        var source = new TaskCompletionSource<List<TSelector>>(TaskCreationOptions.RunContinuationsAsynchronously);

        DatabaseWorkitem workItem = new(
            async db =>
            {
                DbSet<TData> set = setSelector(db);
                IQueryable<TSelector> selector = converter(set);
                List<TSelector> result = await selector.ToListAsync();
                source.SetResult(result);
            },
            ex => source.SetException(ex));

        RunItem(workItem);

        return source.Task;
    }

    private async Task ProcessItems()
    {
        await Task.Yield();

        foreach (DatabaseWorkitem workitem in _workitems.GetConsumingEnumerable())
        {
            try
            {
                _dataBase ??= await CreateDatabase();

                await workitem.Worker(_dataBase).ConfigureAwait(false);

                await _dataBase.SaveChangesAsync();
            }
            catch (Exception e)
            {
                workitem.ErrorReporter(e);
            }

            if (_dataBase is null || _workitems.Count != 0)
            {
                continue;
            }

            await _dataBase.DisposeAsync().ConfigureAwait(false);
            _dataBase = null;
        }
    }

    private async ValueTask<SpoolDataBase> CreateDatabase()
    {
        SpoolDataBase dataBase = new(
            new DbContextOptionsBuilder<SpoolDataBase>()
               .UseSqlite(new SqliteConnectionStringBuilder
                          {
                              Pooling = true,
                              DataSource = Path.Combine(DatabaseDic, "spools.db")
                          }.ConnectionString)
               .Options);

        if (_needMigration)
        {
            await dataBase.Database.MigrateAsync();
            _needMigration = false;
        }

        await dataBase.Database.ExecuteSqlAsync($"VACUUM;");

        return dataBase;
    }
}