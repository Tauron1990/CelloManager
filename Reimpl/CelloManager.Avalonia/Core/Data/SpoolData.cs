using System;
using Be.Vlaanderen.Basisregisters.Generators.Guid;

namespace CelloManager.Core.Data;

public sealed record SpoolData(string Id, string Name, string Category, int Amount, int NeedAmount)
{
    private static readonly Guid _namespace = new("1411CE20-52DF-443B-83D1-B3057FFE824F");
    
    public static string CreateId(string name, string category)
    {
        return Deterministic.Create(_namespace, $"{name}-{category}").ToString("N");
    }

    public static SpoolData New(string name, string category, int amount) 
        => new(CreateId(name, category), name, category, amount, -1);
}