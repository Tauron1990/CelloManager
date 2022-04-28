using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CelloManager.Avalonia.Core.Data;
using CelloManager.Avalonia.ViewModels.SpoolDisplay;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
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
            set => this.RaiseAndSetIfChanged(ref _currentTab, value);
        }

        public string ErrorSimple => _errorSimple.Value;

        public string ErrorFull => _errorFull.Value;
        
        public IObservableCollection<TabViewModel> Tabs { get; } = new ObservableCollectionExtended<TabViewModel>();

        public MainWindowViewModel(ErrorDispatcher errors)
        {
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
            
            _tabs.Add(_modelScope.GetService<SpoolDisplayViewModel>());
        }
        
        public ValueTask DisposeAsync()
        {
            _subscriptions.Dispose();
            _tabs.Clear();
            GC.SuppressFinalize(this);
            return _modelScope.DisposeAsync();
        }
    }
}