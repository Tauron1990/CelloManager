using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Threading;
using CelloManager.Core.DataOperators;
using CelloManager.Core.Logic;
using DynamicData;
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
            .ObserveOn(RxApp.MainThreadScheduler)
            .SelectUpdate(m => new SpoolViewModel(m))
            .DisposeMany()
            .Bind(Spools)
            .Subscribe();

    }

    public void Dispose() => _subscription.Dispose();
}

public sealed class SpoolViewModel : ViewModelBase, IDisposable, IUpdateAware<ReadySpoolModel>
{
    private ReadySpoolModel _model;
    private string _largeName;
    private string _smallName;
    private int _amount;
    private int _needAmount;
    private bool _needAmountSet;

    public ReadySpoolModel Model
    {
        get => _model;
        private set => this.RaiseAndSetIfChanged(ref _model, value);
    }

    public string LargeName
    {
        get => _largeName;
        private set => this.RaiseAndSetIfChanged(ref _largeName, value);
    }

    public string SmallName
    {
        get => _smallName;
        private set => this.RaiseAndSetIfChanged(ref _smallName, value);
    }

    public int Amount
    {
        get => _amount;
        private set => this.RaiseAndSetIfChanged(ref _amount, value);
    }

    public int NeedAmount
    {
        get => _needAmount;
        private set => this.RaiseAndSetIfChanged(ref _needAmount, value);
    }

    public bool NeedAmountSet
    {
        get => _needAmountSet;
        private set => this.RaiseAndSetIfChanged(ref _needAmountSet, value);
    }

    public ReactiveCommand<Unit, Unit> Increment { get; }
    
    public ReactiveCommand<Unit, Unit> Decrement { get; }

    public SpoolViewModel(ReadySpoolModel model)
    {
        UpdateModel(model);

        Increment = ReactiveCommand.CreateFromObservable(
            () => Model.RunIncrement(),
            Observable.Return(model.Amount != int.MaxValue));

        Decrement = ReactiveCommand.CreateFromObservable(
            () => Model.RunDecrement(),
            Observable.Return(model.Amount > 0));
    }

    public void Dispose()
    {
        Increment.Dispose();
        Decrement.Dispose();
    }

    public void Update(ReadySpoolModel model) => UpdateModel(model);

    [MemberNotNull(nameof(LargeName))]
    [MemberNotNull(nameof(SmallName))]
    [MemberNotNull(nameof(Model))]
    
    [MemberNotNull(nameof(_largeName))]
    [MemberNotNull(nameof(_smallName))]
    [MemberNotNull(nameof(_model))]
    
    private void UpdateModel(ReadySpoolModel model)
    {

        Model = model;

        Model = model;
        Amount = model.Amount;
        NeedAmount = model.NeedAmount;
        NeedAmountSet = model.NeedAmountSet;

        var str = model.Name.AsSpan();
        int largeCount = model.Name.Count(char.IsDigit) + model.Name.Count(c => c == ',');

        if(largeCount == 0)
        {
            LargeName = model.Name;
            SmallName = string.Empty;
        }
        else
        {
            LargeName = new string(str[..largeCount]);
            SmallName = new string(str[largeCount..]);
        }
#pragma warning disable CS8774
    }
#pragma warning restore CS8774
}