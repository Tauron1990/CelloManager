using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CelloManager.Avalonia.Core.Data;
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

        private int _currentTab;

        public int CurrentTab
        {
            get => _currentTab;
            [UsedImplicitly]
            set => this.RaiseAndSetIfChanged(ref _currentTab, value);
        }

        public string ErrorSimple => _errorSimple.Value;

        public string ErrorFull => _errorFull.Value;
        
        public IObservableCollection<TabViewModel> Tabs { get; } = new ObservableCollectionExtended<TabViewModel>();

        public ReactiveCommand<Unit, Unit> Edit { get; }
        
        public ReactiveCommand<Unit, Unit> Import { get; }
        
        public ReactiveCommand<Unit, Unit> Order { get; }
        
        public ReactiveCommand<Unit, Unit> Orders { get; }


        public MainWindowViewModel(ErrorDispatcher errors)
        {
            var currentTans = _tabs.Connect().QueryWhenChanged().Publish().RefCount();
            
            Edit = ReactiveCommand.Create(() => { }).DisposeWith(_subscriptions);
            Import = ReactiveCommand.Create(() => { }).DisposeWith(_subscriptions);
            Order = ReactiveCommand.Create(() => { }).DisposeWith(_subscriptions);
            Orders = ReactiveCommand.Create(() => { }).DisposeWith(_subscriptions);
            
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

            Tabs.AsObservableChangeSet()
                .ObserveOn(RxApp.MainThreadScheduler)
                .OnItemAdded(m => CurrentTab = Tabs.IndexOf(m))
                .Subscribe()
                .DisposeWith(_subscriptions);

            _tabs.Connect()
                .OnItemRemoved(m =>
                {
                    if (m is IDisposable disposable)
                        disposable.Dispose();
                })
                .Subscribe()
                .DisposeWith(_subscriptions);

            _tabs.Add(_modelScope.GetService<SpoolDisplayViewModel>());
        }

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