using System.Reactive;
using System.Reactive.Linq;
using CelloManager.Core.Logic;
using ReactiveUI;

namespace CelloManager.ViewModels.Editing;

public class NewSpoolEditorViewModel : SpoolEditorViewModelBase
{
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

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
}