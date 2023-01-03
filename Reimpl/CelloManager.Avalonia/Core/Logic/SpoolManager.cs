using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CelloManager.Core.Comp;
using CelloManager.Core.Data;
using DynamicData;
using DynamicData.Alias;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CelloManager.Core.Logic;

public sealed record ValidateNameRequest(string? Name, string? Category);

public sealed record OldValidateNameRequest(ReadySpoolModel Old, string? Name, string? Category);

public sealed class SpoolManager
{
    private readonly SpoolRepository _repository;
    public IObservable<IGroupChangeSet<ReadySpoolModel, string, string>> CurrentSpools { get; }

    public IObservable<IChangeSet<string, string>> KnowenCategorys { get; }

    public SpoolManager(SpoolRepository repository)
    {
        _repository = repository;
        CurrentSpools = repository.Spools
            .Select(set => new ReadySpoolModel(set, repository))
            .Sort(ReadySpoolSorter.CategorySorter)
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
                return id == r.Old.Data.Id || _repository.ValidateName(r.Name, r.Category);
            });

    public bool Exist(string name, string category)
        => _repository.LookUp(name, category).HasValue;
    
    public IObservable<bool> CanDelete(ReadySpoolModel model)
        => _repository.Spools.QueryWhenChanged().Select(l => l.Keys.Contains(model.Data.Id));
    

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
        
        var id = SpoolData.CreateId(name ?? string.Empty, category ?? string.Empty);

        if (id == old.Id) 
            _repository.UpdateSpool(old with { Amount = amount, NeedAmount = needAmount });
        else if(_repository.ValidateName(name, category))
            _repository.Edit(u =>
            {
                u.Remove(old);
                u.AddOrUpdate(SpoolData.New(name, category, amount) with { NeedAmount = needAmount });
            });
    }

    public async Task<Exception?> ImportSpools(string path)
    {
        try
        {
            if(!File.Exists(path)) return null;

            await using var database = new CoreDatabase();
        
        
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = path
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
        catch (Exception e)
        {
            return e;
        }
    }

    public void Delete(ReadySpoolModel model)
        => _repository.Delete(model.Data);
}