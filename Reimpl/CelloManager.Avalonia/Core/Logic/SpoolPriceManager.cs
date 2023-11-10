using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CelloManager.Core.Data;
using DynamicData;
using DynamicData.Kernel;

namespace CelloManager.Core.Logic;

public class SpoolPriceManager
{
    private readonly SpoolRepository _repository;
    
    public IObservable<double> CompledPrice { get; }

    public Optional<PriceDefinition> Find(string name)
        => _repository.TryFindPrice(name);

    public void Update(PriceDefinition? definition)
#pragma warning disable MA0134
        => Task.Run(() => _repository.UpdatePrice(definition));
#pragma warning restore MA0134
    
    public SpoolPriceManager(SpoolRepository repository)
    {
        _repository = repository;
        CompledPrice = repository
            .Prices.Group(d => d.Name)
            .Synchronize()
            .Transform(g => new SpoolPriceModel(g, repository.Spools))
            .DisposeMany()
            .QueryWhenChanged()
            .Select(query => query.Items.Select(m => m.Price.StartWith(0)).CombineLatest(list => list.Sum()))
            .Switch();
    }
}