using System.Collections.Immutable;
using CelloManager.Data;
using Newtonsoft.Json;

namespace CelloManager.Core.Data.Converter;

public static class Mapper
{
    public static SpoolDataDb ToDatabase(this SpoolData data)
        => new()
           {
               Id = data.Id,
               Name = data.Name,
               Category = data.Category,
               Amount = data.Amount,
               NeedAmount = data.NeedAmount,
           };

    public static SpoolData FromDatabase(this SpoolDataDb data) 
        => new(data.Id, data.Name, data.Category, data.Amount, data.NeedAmount);

    public static PendingOrderDb ToDatabase(this PendingOrder order)
        => new()
           {
               Id = order.Id,
               Time = order.Time,
               Spools = JsonConvert.SerializeObject(order.Spools),
           };

    public static PendingOrder FromDatabase(this PendingOrderDb data)
        => new(data.Id, JsonConvert.DeserializeObject<ImmutableList<OrderedSpoolList>>(data.Spools) ?? ImmutableList<OrderedSpoolList>.Empty, data.Time);

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