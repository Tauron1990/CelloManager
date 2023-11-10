using System.Collections.Immutable;

namespace CelloManager.Core.Data;

public sealed record OrderedSpoolList(string Category, ImmutableList<OrderedSpool> Spools);