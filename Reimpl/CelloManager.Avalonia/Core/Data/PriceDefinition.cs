using System;
using Be.Vlaanderen.Basisregisters.Generators.Guid;

namespace CelloManager.Core.Data;

public record PriceDefinition(string Id, string Name, double Price, double Lenght) : IHasId
{
    private static readonly Guid Namespace = new Guid("810843A9-A195-45EB-859D-2DEF0B8B059D");

    public static string CreateId(string name)
        => Deterministic.Create(Namespace, name).ToString("N");
    
    public static PriceDefinition New(string name, double price, double lenght)
        => new(CreateId(name), name, price, lenght);
}