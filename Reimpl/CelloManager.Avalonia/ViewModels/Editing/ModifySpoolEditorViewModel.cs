﻿using System.Reactive;
using System.Reactive.Linq;
using CelloManager.Core.Logic;
using ReactiveUI;

namespace CelloManager.ViewModels.Editing;

public class ModifySpoolEditorViewModel : SpoolEditorViewModelBase
{
    public static ViewModelBase Create(ReadySpoolModel model, SpoolManager manager)
        => new ModifySpoolEditorViewModel(model, manager);


    public ReactiveCommand<Unit, Unit> Save { get; }

    public ReactiveCommand<Unit, Unit> Delete { get; }

    private ModifySpoolEditorViewModel(ReadySpoolModel model, SpoolManager manager)
        : base(manager)
    {
        Name = model.Name;
        Category = model.Category;
        Amount = model.Amount;
        NeedAmount = model.NeedAmount;
        
        Save = ReactiveCommand.Create(
            () => manager.UpdateSpool(model, Name, Category, Amount, NeedAmount),
            manager.ValidateModifyName(
                    this.WhenAnyValue(m => m.Name, m => m.Category)
                    .Select(p => new OldValidateNameRequest(model, p.Item1, p.Item2)))
                .ObserveOn(RxApp.MainThreadScheduler));
        
        Delete = ReactiveCommand.Create(
            () => manager.Delete(model),
            manager.CanDelete(model)
                .ObserveOn(RxApp.MainThreadScheduler));
    }
}