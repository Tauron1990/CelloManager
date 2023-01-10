using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CelloManager.Core.Logic;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
using ReactiveUI;

namespace CelloManager.ViewModels.SpoolDisplay;

public sealed class SpoolGroupViewModel : ViewModelBase, ITabInfoProvider, IDisposable
{
    private readonly IDisposable _subscription;
    
    public bool CanClose => false;
    
    public string Title { get; }


    public IObservableCollection<SpoolViewModel> Spools { get; } = new ObservableCollectionExtended<SpoolViewModel>();

    public SpoolGroupViewModel(string category, IConnectableCache<ReadySpoolModel, string> spools)
    {
        Title = category;

        _subscription = spools.Connect()
            .Sort(ReadySpoolSorter.ModelSorter)
            .Select(m => new SpoolViewModel(m))
            .DisposeMany()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(Spools)
            .Subscribe();

    }

    public void Dispose() => _subscription.Dispose();
}

public sealed class SpoolViewModel : ViewModelBase, IDisposable
{
    public ReadySpoolModel Model { get; }
    
    public string LargeName { get; }
    
    public string SmallName { get; }
    
    public int Amount { get; }

    public int NeedAmount { get; }

    public bool NeedAmountSet { get; }

    public ReactiveCommand<Unit, Unit> Increment { get; }
    
    public ReactiveCommand<Unit, Unit> Decrement { get; }

    public SpoolViewModel(ReadySpoolModel model)
    {
        Model = model;
        Amount = model.Amount;
        NeedAmount = model.NeedAmount;
        NeedAmountSet = model.NeedAmountSet;

        Increment = ReactiveCommand.Create(
            model.RunIncrement,
            Observable.Return(model.Amount != int.MaxValue));

        Decrement = ReactiveCommand.Create(
            model.RunDecrement,
            Observable.Return(model.Amount > 0));
        
        var str = model.Name.AsSpan();
        var largeCount = model.Name.Count(char.IsDigit) + model.Name.Count(c => c == ',');
        
        if (largeCount == 0)
        {
            LargeName = model.Name;
            SmallName = string.Empty;
        }
        else
        {
            LargeName = new string(str[..largeCount]);
            SmallName = new string(str[largeCount..]);
        }
    }

    public void Dispose()
    {
        Increment.Dispose();
        Decrement.Dispose();
    }
}