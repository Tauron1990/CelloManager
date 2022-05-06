using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CelloManager.Avalonia.Core.Data;
using CelloManager.Avalonia.Core.DebugHelper;
using CelloManager.Avalonia.Core.Logic;
using CelloManager.Avalonia.ViewModels.Editing;
using CelloManager.Avalonia.ViewModels.Orders;
using CelloManager.Avalonia.ViewModels.SpoolDisplay;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
using JetBrains.Annotations;
using ReactiveUI;

namespace CelloManager.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IAsyncDisposable
    {
        private readonly AppServiceProvider.Scope _modelScope = App.ServiceProvider.CreateScope();
        private readonly SourceList<ViewModelBase> _tabs = new();
        private readonly CompositeDisposable _subscriptions = new();
        private readonly ObservableAsPropertyHelper<string> _errorSimple;
        private readonly ObservableAsPropertyHelper<string> _errorFull;

        public string ErrorSimple => _errorSimple.Value;

        public string ErrorFull => _errorFull.Value;
        
        public IObservableCollection<TabViewModel> Tabs { get; } = new ObservableCollectionExtended<TabViewModel>();

        public ReactiveCommand<Unit, Unit> Edit { get; }
        
        public ReactiveCommand<Unit, Unit> Import { get; }
        
        public ReactiveCommand<Unit, Unit> Order { get; }
        
        public ReactiveCommand<Unit, Unit> Orders { get; }


        public MainWindowViewModel(ErrorDispatcher errors)
        {
            var currentTabs = _tabs.Connect().QueryWhenChanged().Publish().RefCount();
            var orderManager = _modelScope.GetService<OrderManager>();
            
            Edit = ReactiveCommand.Create
                (
                    () => _tabs.Add(_modelScope.GetService<EditTabViewModel>()),
                    ContainsViewModel<EditTabViewModel>(currentTabs)
                )
                .DisposeWith(_subscriptions);
            
            Orders = ReactiveCommand.Create(
                    () => _tabs.Add(_modelScope.GetService<OrderDisplayViewModel>()),
                    ContainsViewModel<OrderDisplayViewModel>(currentTabs))
                .DisposeWith(_subscriptions);
            
            Import = ReactiveCommand.Create(() => { }).DisposeWith(_subscriptions);
            
            Order = ReactiveCommand.CreateFromObservable(
                    () => Observable.Return(orderManager.PlaceOrder())
                        .Where(isPlaced => isPlaced)
                        .SelectMany(_ => Orders.Execute()),
                    orderManager.CanOrder.ObserveOn(RxApp.MainThreadScheduler))
                .DisposeWith(_subscriptions);

            _errorFull = errors.Errors
                .Select(e => e.ToString())
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, m => m.ErrorFull)
                .DisposeWith(_subscriptions);

            _errorSimple = errors.Errors
                .Select(e => e.Message)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, m => m.ErrorSimple)
                .DisposeWith(_subscriptions);

            _tabs.Connect()
                .Select(t => TabViewModel.Create(t, _tabs))
                .DisposeMany()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(Tabs)
                .Subscribe()
                .DisposeWith(_subscriptions);

            _tabs.Add(_modelScope.GetService<SpoolDisplayViewModel>());
        }

        private IObservable<bool> ContainsViewModel<TModel>(IObservable<IReadOnlyCollection<ViewModelBase>> query)
            => query.Select(l => l.All(vm => vm.GetType() != typeof(TModel))).ObserveOn(RxApp.MainThreadScheduler);

        public ValueTask DisposeAsync()
        {
            _subscriptions.Dispose();
            _tabs.Clear();
            _tabs.Dispose();
            GC.SuppressFinalize(this);
            return _modelScope.DisposeAsync();
        }
    }
}