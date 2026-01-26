using System;
using Be.Vlaanderen.Basisregisters.Generators.Guid;

namespace CelloManager.Core.Data;

public record PriceDefinition(string Id, string Name, double Price, double Lenght) : IHasId
{
    private static readonly Guid Namespace = new(0x810843A9, 0xA195, 0x45EB, 0x85, 0x9D, 0x2D, 0xEF, 0xB, 0x8B, 0x5, 0x9D) /* 810843A9-A195-45EB-859D-2DEF0B8B059D */;

    public static string CreateId(string name)
        => Deterministic.Create(Namespace, name).ToString("N");

    public static PriceDefinition New(string name, double price, double lenght)
        => new(CreateId(name), name, price, lenght);
}