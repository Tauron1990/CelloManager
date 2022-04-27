using System;

namespace CelloManager.Avalonia.Core.Data;

public sealed record PendingOrder(string Id, string SpoolId, int Amount)
{
    public static PendingOrder New(string spoolId, int amount)
        => new(Guid.NewGuid().ToString("N"), spoolId, amount);
}