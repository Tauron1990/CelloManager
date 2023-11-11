using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CelloManager.Core.Comp;
using CelloManager.Core.Data;
using DynamicData;
using DynamicData.Alias;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CelloManager.Core.Logic;

public sealed class SpoolManager
{
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerOptions.Default)
    {
        WriteIndented = true,
    };
    private readonly SpoolRepository _repository;
    public IObservable<IGroupChangeSet<ReadySpoolModel, string, string>> CurrentSpools { get; }

    public IObservable<int> Count => _repository.SpoolCount;

    public IObservable<IChangeSet<string, string>> KnowenCategorys { get; }

    public SpoolManager(SpoolRepository repository)
    {
        _repository = repository;
        CurrentSpools = repository.Spools
            .Select(set => new ReadySpoolModel(set, repository))
            .Sort(ReadySpoolSorter.CategoryModelSorter)
            .Group(m => m.Category);

        KnowenCategorys = CurrentSpools.Select(g => g.Key);
    }

    public IObservable<bool> ValidateName(IObservable<ValidateNameRequest> requests) =>
        requests
            .CombineLatest(
                _repository.Spools
                    .QueryWhenChanged()
                    .Select(_ => Unit.Default)
                    .StartWith(Unit.Default),
                (r, _) => r)
            .Select(r => _repository.ValidateName(r.Name, r.Category));

    public IObservable<bool> ValidateModifyName(IObservable<OldValidateNameRequest> requests) =>
        requests
            .CombineLatest(
                _repository.Spools
                    .QueryWhenChanged()
                    .Select(_ => Unit.Default)
                    .StartWith(Unit.Default),
                (r, _) => r)
            .Select(r =>
            {
                var id = SpoolData.CreateId(r.Name ?? string.Empty, r.Category ?? string.Empty);
                return string.Equals(id, r.Old.Data.Id, StringComparison.Ordinal) || _repository.ValidateName(r.Name, r.Category);
            });

    public bool Exist(string name, string category)
        => _repository.LookUp(name, category).HasValue;
    
    public IObservable<bool> CanDelete(ReadySpoolModel model)
        => _repository.Spools.QueryWhenChanged().Select(l => l.Keys.Contains(model.Data.Id, StringComparer.Ordinal));
    

    public void CreateSpool(string? name, string? category, int amount, int needAmount)
    {
        if(!_repository.ValidateName(name, category)) return;

        if (amount < 0)
            amount = 0;

        var data = SpoolData.New(name, category, amount);
        if (needAmount > 0)
            data = data with { NeedAmount = needAmount };

        _repository.UpdateSpool(data);
    }

    public void UpdateSpool(SpoolData old, string? name, string? category, int amount, int needAmount)
    {
        if (needAmount <= 0)
            needAmount = -1;
        
        string id = SpoolData.CreateId(name ?? string.Empty, category ?? string.Empty);

        if (string.Equals(id, old.Id, StringComparison.Ordinal)) 
            _repository.UpdateSpool(old with { Amount = amount, NeedAmount = needAmount });
        else if(_repository.ValidateName(name, category))
            _repository.Edit(u =>
            {
                u.Remove(old);
                u.AddOrUpdate(SpoolData.New(name, category, amount) with { NeedAmount = needAmount });
            });
    }

    public async Task<Exception?> ExportToJson(IStorageFile path)
    {
        try
        {
            var array = _repository.SpoolItems.ToArray();

            var stream = await path.OpenWriteAsync().ConfigureAwait(false);
            await using (stream.ConfigureAwait(false))
            {
                await JsonSerializer.SerializeAsync(stream, array, _serializerOptions).ConfigureAwait(false);

            return null;
            }
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<Exception?> ImportFromJson(IStorageFile path)
    {
        try
        {
            var stream = await path.OpenReadAsync().ConfigureAwait(false);
            await using (stream.ConfigureAwait(false))
            {
                var array = await JsonSerializer.DeserializeAsync<SpoolData[]>(stream).ConfigureAwait(false);

            if(array is null)
                throw new InvalidOperationException("Lesen der Daten Fehlgeschlagen");

            foreach (SpoolData spool in array)
            {
                var old = _repository.LookUp(spool.Name, spool.Category);
            
                if(old.HasValue)
                    UpdateSpool(old.Value, spool.Name, spool.Category, spool.Amount, spool.NeedAmount);
                else
                    CreateSpool(spool.Name, spool.Category, spool.Amount, spool.NeedAmount); 
            }
            
            return null;
            }
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<Exception?> ImportFromLegacy(IStorageFile path)
    {
        try
        {
            if(!File.Exists(path.Path.LocalPath)) return null;

            var database = new CoreDatabase();
            await using (database.ConfigureAwait(false))
            {
                var builder = new SqliteConnectionStringBuilder
            {
                DataSource = path.Path.LocalPath,
            };

            database.Database.SetConnectionString(builder.ConnectionString);

            await foreach (var spool in database.CelloSpools)
            {
                var old = _repository.LookUp(spool.Name, spool.Type);
            
                if(old.HasValue)
                    UpdateSpool(old.Value, spool.Name, spool.Type, spool.Amount, spool.Neededamount);
                else
                    CreateSpool(spool.Name, spool.Type, spool.Amount, spool.Neededamount);
            }
        
            return null;
            }
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public void Delete(ReadySpoolModel model)
        => _repository.Delete(model.Data);
}