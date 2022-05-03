using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CelloManager.Avalonia.Core.Logic;
using DynamicData;
using JetBrains.Annotations;
using ReactiveUI;

namespace CelloManager.Avalonia.ViewModels.Editing;

public class NewSpoolEditorViewModel : ViewModelBase, IDisposable
{
    private readonly CompositeDisposable _subscriptions = new();
    private string? _name;
    private string? _category;
    private int _amount;
    private int _needAmount;
    private int? _popupSelection;

    public string? Name
    {
        get => _name;
        [UsedImplicitly]
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string? Category
    {
        get => _category;
        [UsedImplicitly]
        set => this.RaiseAndSetIfChanged(ref _category, value);
    }

    public int Amount
    {
        get => _amount;
        [UsedImplicitly]
        set => this.RaiseAndSetIfChanged(ref _amount, value);
    }

    public int NeedAmount
    {
        get => _needAmount;
        [UsedImplicitly]
        set => this.RaiseAndSetIfChanged(ref _needAmount, value);
    }

    public ReadOnlyObservableCollection<string> KnowenCategorys { get; }

    public int? PopupSelection
    {
        get => _popupSelection;
        set => this.RaiseAndSetIfChanged(ref _popupSelection, value);
    }

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    
    public NewSpoolEditorViewModel(SpoolManager manager)
    {
        SaveCommand = ReactiveCommand.Create
        (
            () => manager.CreateSpool(Name, Category, Amount, NeedAmount),
            manager
                .ValidateName(
                    this.WhenAnyValue(m => m.Name, m => m.Category)
                        .Select(t => new ValidateNameRequest(t.Item1, t.Item2)))
                .ObserveOn(RxApp.MainThreadScheduler)
        );

        manager.KnowenCategorys
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var categorys)
            .Subscribe()
            .DisposeWith(_subscriptions);

        this.WhenAnyValue(m => m.Category)
            .Subscribe(nc =>
            {
                if (!string.IsNullOrWhiteSpace(nc) && KnowenCategorys?.Contains(nc) == true)
                    PopupSelection = KnowenCategorys.IndexOf(nc);
                else
                    PopupSelection = null;
            })
            .DisposeWith(_subscriptions);

        this.WhenAnyValue(m => m.PopupSelection)
            .Where(s => s >= 0)
            .Subscribe(s => Category = KnowenCategorys?.ElementAt(s ?? 0))
            .DisposeWith(_subscriptions);
            
        
        KnowenCategorys = categorys;
    }

    public void Dispose()
    {
        _subscriptions.Dispose();
        SaveCommand.Dispose();
        
        GC.SuppressFinalize(this);
    }
}