using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using CelloManager.Data;

namespace CelloManager.Core.Data.Converter;

[RequiresUnreferencedCode("Database (EF Core)")]
public static class Mapper
{
    public static SpoolDataDb ToDatabase(this SpoolData data)
        => new()
           {
               Id = data.Id,
               Name = data.Name,
               Category = data.Category,
               Amount = data.Amount,
               NeedAmount = data.NeedAmount
           };

    public static SpoolData FromDatabase(this SpoolDataDb data)
        => new(data.Id, data.Name, data.Category, data.Amount, data.NeedAmount);

    [RequiresUnreferencedCode("Access to Database (EF Core)")]
    public static PendingOrderDb ToDatabase(this PendingOrder order)
        => new()
           {
               Id = order.Id,
               Time = order.Time,
               Spools = JsonSerializer.Serialize(order.Spools, SerializerContext.Default.ImmutableListOrderedSpoolList),
           };

    public static PendingOrder FromDatabase(this PendingOrderDb data)
        => new(data.Id, JsonSerializer.Deserialize<ImmutableList<OrderedSpoolList>>(data.Spools, SerializerContext.Default.ImmutableListOrderedSpoolList) 
                     ?? ImmutableList<OrderedSpoolList>.Empty, data.Time);

    public static PriceDefinitionDb ToDatabase(this PriceDefinition data)
        => new()
           {
               Id = data.Id,
               Name = data.Name,
               Lenght = data.Lenght,
               Price = data.Price,
           };

    public static PriceDefinition FromDatabase(this PriceDefinitionDb data)
        => new(data.Id, data.Name, data.Price, data.Lenght);
}