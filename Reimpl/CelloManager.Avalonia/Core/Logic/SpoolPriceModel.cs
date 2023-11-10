using System;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using CelloManager.Core.Data;
using DynamicData;
using DynamicData.Aggregation;

namespace CelloManager.Core.Logic;

public sealed class SpoolPriceModel : IDisposable
{
    //public string Key { get; }
    
    public IObservable<double> Price { get; }

    public SpoolPriceModel(IGroup<PriceDefinition, string, string> group, IObservable<IChangeSet<SpoolData, string>> spools)
    {
        //Key = group.Key;
        
        Price = spools
            .InvalidateWhen(group.Cache.Connect())
            .Synchronize()
            .Filter(s => string.Equals(s.Category, group.Key, StringComparison.Ordinal))
            .Transform(d => (Data: d, Price: group.Cache.Items.SingleOrDefault()))
            .Filter(p => p.Price is not null && p.Price.Price > 0 && p.Price.Lenght > 0)
            .Transform(p => (p.Data.Amount, Weith:ExtractFromName(p.Data.Name), p.Price))
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            .Filter(p => p.Amount > 0 && p.Weith != -1)
            .Sum(p => p.Weith / 100 * p.Price!.Lenght * p.Amount * p.Price.Price);
    }

    private double ExtractFromName(string name)
    {
        var str = name.AsSpan();
        var largeCount = name.Count(char.IsDigit) + name.Count(c => c == ',');

        if(double.TryParse(str[..largeCount], NumberStyles.Any, CultureInfo.CurrentUICulture, out double number))
            return number;

        return -1;
    }
    
    public void Dispose()
    {
        
    }
}