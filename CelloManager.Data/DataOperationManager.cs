using System.Collections.Concurrent;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CelloManager.Data;

public sealed class DataOperationManager
{
    public static readonly DataOperationManager Manager = new();

    private readonly BlockingCollection<DatabaseWorkitem> _workitems = new();
    private readonly Task _workerThread;
    private SpoolDataBase? _dataBase;
    
    private DataOperationManager() => _workerThread = Task.Run(ProcessItems);

    public async ValueTask Shutdown()
    {
        _workitems.CompleteAdding();
        await _workerThread.ConfigureAwait(false);
    }

    public void RunItem(DatabaseWorkitem item)
    {
        if(_workitems.IsAddingCompleted) return;
        _workitems.Add(item);
    }

    public Task<List<TSelector>> Query<TData, TSelector>(
        Func<SpoolDataBase, DbSet<TData>> setSelector,
        Func<IQueryable<TData>, IQueryable<TSelector>> converter)
        where TData : class
    {
        var source = new TaskCompletionSource<List<TSelector>>(TaskCreationOptions.RunContinuationsAsynchronously);

        var workItem = new DatabaseWorkitem(
            async db =>
            {
                var set = setSelector(db);
                var selector = converter(set);
                var result = await selector.ToListAsync();
                source.SetResult(result);
            },
            ex => source.SetException(ex));
        
        RunItem(workItem);
        
        return source.Task;
    }
    
    private async Task ProcessItems()
    {
        await Task.Yield();

        foreach (var workitem in _workitems.GetConsumingEnumerable())
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
            
            if (_dataBase is null || _workitems.Count != 0) continue;
                
            await _dataBase.DisposeAsync().ConfigureAwait(false);
            _dataBase = null;
        }
    }

    private static async ValueTask<SpoolDataBase> CreateDatabase()
    {
        var databaseDic = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Tauron", "CelloManager");
        if (!Directory.Exists(databaseDic))
            Directory.CreateDirectory(databaseDic);


        var dataBase = new SpoolDataBase(
            new DbContextOptionsBuilder<SpoolDataBase>()
               .UseSqlite(new SqliteConnectionStringBuilder
                          {
                              DataSource = Path.Combine(databaseDic, "spools.db"),
                          }.ConnectionString)
               .Options);

        await dataBase.Database.MigrateAsync();
        await dataBase.Database.ExecuteSqlAsync($"VACUUM;");

        return dataBase;
    }
}