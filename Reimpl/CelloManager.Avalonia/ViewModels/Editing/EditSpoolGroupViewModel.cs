using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CelloManager.Core.Data;
using CelloManager.Core.Logic;
using DynamicData.Kernel;
using JetBrains.Annotations;
using ReactiveUI;

namespace CelloManager.ViewModels.Editing;

public class EditSpoolGroupViewModel : ViewModelBase, IActivatableViewModel, IDisposable
{
    private PriceDefinition? _start;

    private EditSpoolGroupViewModel(
        ReadOnlyObservableCollection<ReadySpoolModel> spools, Action<ReadySpoolModel> modelSelected,
        SpoolManager spoolManager, SpoolPriceManager priceManager, string category)
    {
        Spools = spools;

        this.WhenActivated(Init);

        Optional<PriceDefinition> potentialPrice = priceManager.Find(category);
        potentialPrice.IfHasValue(
                           def =>
                           {
                               _start = def;
                               Lenght = def.Lenght;
                               Price = def.Price;
                           })
                      .Else(
                           () =>
                           {
                               _start = null;
                               Lenght = -1;
                               Price = -1;
                           });


        DeleteAll = ReactiveCommand.Create(DeleteAllImpl);

        IEnumerable<IDisposable> Init()
        {
            Selected = null;

            yield return this.WhenAnyValue(m => m.Selected).Where(m => m is not null).Subscribe(modelSelected!);

            yield return this.WhenAny(
                                  m => m.Lenght,
                                  m => m.Price,
                                  (lenght, price) => PriceDefinition.New(category, price.Value, lenght.Value))
                             .StartWith(_start)
                             .DistinctUntilChanged()
                             .Subscribe(priceManager.Update);
        }

        void DeleteAllImpl()
        {
            #pragma warning disable MA0134
            Task.Run(() =>
                     {
                         foreach (ReadySpoolModel model in spools.ToArray())
                         {
                             spoolManager.Delete(model);
                         }
                     });
            #pragma warning restore MA0134
        }
    }

    public IEnumerable<ReadySpoolModel> Spools { get; }

    public ReactiveCommand<Unit, Unit> DeleteAll { get; }

    public double Price
    {
        get;
        [UsedImplicitly]
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public double Lenght
    {
        get;
        [UsedImplicitly]
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public ReadySpoolModel? Selected
    {
        get;
        [UsedImplicitly] set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public ViewModelActivator Activator { get; } = new();

    void IDisposable.Dispose()
    {
        DeleteAll.Dispose();
        GC.SuppressFinalize(this);
    }

    public static ViewModelBase Create(
        ReadOnlyObservableCollection<ReadySpoolModel> spools, SpoolManager manager,
        Action<ReadySpoolModel> modelSelected, SpoolPriceManager priceManager, string category)
        => new EditSpoolGroupViewModel(spools, modelSelected, manager, priceManager, category);
}