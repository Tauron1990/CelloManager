using System;
using System.Reactive;
using System.Reactive.Linq;
using CelloManager.Avalonia.Core.Data;
using DynamicData;
using DynamicData.Alias;

namespace CelloManager.Avalonia.Core.Logic;

public sealed record ValidateNameRequest(string? Name, string? Category);

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
            .Group(m => m.Category);

        KnowenCategorys = CurrentSpools.Select(g => g.Key);
    }

    public IObservable<bool> ValidateName(IObservable<ValidateNameRequest> requests)
    {
        return requests
            .CombineLatest(
                _repository.Spools
                    .QueryWhenChanged()
                    .Select(_ => Unit.Default)
                    .StartWith(Unit.Default),
                (r, _) => r)
            .Select(r => _repository.ValidateName(r.Name, r.Category));
    }

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

    public void UpdateSpool(ReadySpoolModel old, string? name, string? category, int amount, int needAmount)
    {
        
    }
}