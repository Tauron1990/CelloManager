using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CelloManager.Core.Data;

public sealed record OrderedSpoolList(string Category, ImmutableList<OrderedSpool> Spools);

public sealed record OrderedSpool(string SpoolId, string Name, int Amount);

public sealed record PendingOrder(string Id, ImmutableList<OrderedSpoolList> Spools, DateTimeOffset Time)
{
    public static PendingOrder New(IEnumerable<SpoolData> spools, Func<SpoolData, int> amountSelector)
    {

        return new(
            Guid.NewGuid().ToString("N"),
            ImmutableList<OrderedSpoolList>.Empty
                .AddRange(
                    spools
                        .GroupBy(s => s.Category)
                        .Select(
                            g => new OrderedSpoolList(
                                g.Key,
                                g.OrderBy(d => d.Name)
                                    .Select(sd => new OrderedSpool(sd.Id, sd.Name, amountSelector(sd)))
                                    .ToImmutableList()))),
            DateTimeOffset.Now);
    }
}