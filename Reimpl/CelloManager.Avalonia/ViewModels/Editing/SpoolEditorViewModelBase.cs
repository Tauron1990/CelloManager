using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CelloManager.Avalonia.Core.Logic;
using DynamicData;
using JetBrains.Annotations;
using ReactiveUI;

namespace CelloManager.Avalonia.ViewModels.Editing;

public class SpoolEditorViewModelBase : ViewModelBase, IDisposable
{
    protected readonly CompositeDisposable Subscriptions = new();
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

    public SpoolEditorViewModelBase(SpoolManager manager)
    {
        manager.KnowenCategorys
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var categorys)
            .Subscribe()
            .DisposeWith(Subscriptions);

        this.WhenAnyValue(m => m.Category)
            .Subscribe(nc =>
            {
                if (!string.IsNullOrWhiteSpace(nc) && KnowenCategorys?.Contains(nc) == true)
                    PopupSelection = KnowenCategorys.IndexOf(nc);
                else
                    PopupSelection = null;
            })
            .DisposeWith(Subscriptions);

        this.WhenAnyValue(m => m.PopupSelection)
            .Where(s => s >= 0)
            .Subscribe(s => Category = KnowenCategorys?.ElementAt(s ?? 0))
            .DisposeWith(Subscriptions);
            
        
        KnowenCategorys = categorys;
    }

    public virtual void Dispose()
    {
        Subscriptions.Dispose();
        
        GC.SuppressFinalize(this);
    }
}