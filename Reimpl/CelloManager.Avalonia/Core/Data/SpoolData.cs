using System;
using Be.Vlaanderen.Basisregisters.Generators.Guid;

namespace CelloManager.Core.Data;

public sealed record SpoolData(string Id, string Name, string Category, int Amount, int NeedAmount) : IHasId
{
    private static readonly Guid Namespace = new(0x1411CE20, 0x52DF, 0x443B, 0x83, 0xD1, 0xB3, 0x5, 0x7F, 0xFE, 0x82, 0x4F) /* 1411CE20-52DF-443B-83D1-B3057FFE824F */;

    public static string CreateId(string name, string category) => Deterministic.Create(Namespace, $"{name}-{category}").ToString("N");

    public static SpoolData New(string name, string category, int amount)
        => new(CreateId(name, category), name, category, amount, -1);
}