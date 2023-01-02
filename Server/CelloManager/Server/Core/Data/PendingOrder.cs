using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CelloManager.Avalonia.Core.Data;

public sealed record OrderedSpool(string SpoolId, int Amount);

public sealed record PendingOrder(string Id, ImmutableList<OrderedSpool> Spools, DateTimeOffset Time)
{
    public static PendingOrder New(IEnumerable<OrderedSpool> spools)
        => new(Guid.NewGuid().ToString("N"), ImmutableList<OrderedSpool>.Empty.AddRange(spools), DateTimeOffset.Now);
}