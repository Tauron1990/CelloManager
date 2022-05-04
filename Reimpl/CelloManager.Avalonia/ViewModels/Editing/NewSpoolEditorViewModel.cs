using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CelloManager.Avalonia.Core.Logic;
using ReactiveUI;

namespace CelloManager.Avalonia.ViewModels.Editing;

public class NewSpoolEditorViewModel : SpoolEditorViewModelBase
{
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    
    public NewSpoolEditorViewModel(SpoolManager manager)
        : base(manager)
    {
        SaveCommand = ReactiveCommand.Create
        (
            () => manager.CreateSpool(Name, Category, Amount, NeedAmount),
            manager
                .ValidateName(
                    this.WhenAnyValue(m => m.Name, m => m.Category)
                        .Select(t => new ValidateNameRequest(t.Item1, t.Item2)))
                .ObserveOn(RxApp.MainThreadScheduler)
        ).DisposeWith(Subscriptions);
    }
}