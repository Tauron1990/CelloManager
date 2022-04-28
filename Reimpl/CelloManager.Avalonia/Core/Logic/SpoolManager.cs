using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using CelloManager.Avalonia.Core.Data;
using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Alias;
using DynamicData.Tests;

namespace CelloManager.Avalonia.Core.Logic;

public sealed class SpoolManager
{
    private Dictionary<string, ReadySpoolViewModel> _models = new();

    public IObservable<IGroupChangeSet<ReadySpoolViewModel, string, string>> CurrentSpools { get; }

    public SpoolManager(SpoolRepository repository)
    {
        CurrentSpools = repository.Spools
            .Select(set =>
            {
                var pool = ArrayPool<Change<ReadySpoolViewModel, string>>.Shared;
                var changes = pool.Rent(set.Count);

                try
                {
                    for (int i = 0; i < set.Count; i++)
                    {
                        var data = set.ElementAt<Change<SpoolData, string>>(i);
                        ReadySpoolViewModel model;

                        switch (data.Reason)
                        {
                            case ChangeReason.Add:
                                break;
                            case ChangeReason.Remove:
                                break;
                            case ChangeReason.Update:
                            case ChangeReason.Refresh:
                            case ChangeReason.Moved:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    return (IChangeSet<ReadySpoolViewModel, string>)new ChangeSet<ReadySpoolViewModel, string>(changes.Take(set.Count));
                }
                finally
                {
                    pool.Return(changes);
                }
            })
            .Group(m => m.Category);
    }
}