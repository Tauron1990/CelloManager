using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CelloManager.Core.Logic;
using DynamicData;
using JetBrains.Annotations;
using ReactiveUI;

namespace CelloManager.ViewModels.Editing;

public class SpoolEditorViewModelBase : ViewModelBase, IDisposable
{
    protected readonly CompositeDisposable Subscriptions = new();

    public SpoolEditorViewModelBase(SpoolManager manager)
    {
        manager.KnowenCategorys
               .ObserveOn(RxApp.MainThreadScheduler)
               .Bind(out ReadOnlyObservableCollection<string> categorys)
               .Subscribe()
               .DisposeWith(Subscriptions);

        this.WhenAnyValue(m => m.Category)
            .Subscribe(nc =>
                       {
                           if (!string.IsNullOrWhiteSpace(nc) && KnowenCategorys?.Contains(nc, StringComparer.Ordinal) == true)
                           {
                               PopupSelection = KnowenCategorys.IndexOf(nc, StringComparer.Ordinal);
                           }
                           else
                           {
                               PopupSelection = null;
                           }
                       })
            .DisposeWith(Subscriptions);

        this.WhenAnyValue(m => m.PopupSelection)
            .Where(s => s >= 0)
            .Subscribe(s => Category = KnowenCategorys?.ElementAt(s ?? 0))
            .DisposeWith(Subscriptions);


        KnowenCategorys = categorys;
    }

    public string? Name
    {
        get;
        [UsedImplicitly] set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public string? Category
    {
        get;
        [UsedImplicitly] set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public int Amount
    {
        get;
        [UsedImplicitly] set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public int NeedAmount
    {
        get;
        [UsedImplicitly] set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public ReadOnlyObservableCollection<string> KnowenCategorys { get; }

    public int? PopupSelection
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public virtual void Dispose()
    {
        Subscriptions.Dispose();

        GC.SuppressFinalize(this);
    }
}