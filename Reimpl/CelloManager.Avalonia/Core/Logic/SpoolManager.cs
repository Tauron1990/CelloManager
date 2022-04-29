using System;
using System.Reactive.Linq;
using CelloManager.Avalonia.Core.Data;
using DynamicData;
using DynamicData.Alias;

namespace CelloManager.Avalonia.Core.Logic;

public sealed class SpoolManager
{
    public IObservable<IGroupChangeSet<ReadySpoolModel, string, string>> CurrentSpools { get; }

    public SpoolManager(SpoolRepository repository)
    {
        CurrentSpools = repository.Spools
            .Select(set => new ReadySpoolModel(set, repository))
            .Group(m => m.Category)
            .Publish().RefCount();
    }
}