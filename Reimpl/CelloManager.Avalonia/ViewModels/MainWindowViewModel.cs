using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace CelloManager.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IAsyncDisposable
    {
        private readonly AppServiceProvider.Scope _modelScope = App.ServiceProvider.CreateScope();
        private readonly SourceList<ViewModelBase> _tabs = new();
        private int _currentTab;

        public int CurrentTab
        {
            get => _currentTab;
            set => this.RaiseAndSetIfChanged(ref _currentTab, value);
        }

        public IObservableCollection<TabViewModel> Tabs { get; } = new ObservableCollectionExtended<TabViewModel>();

        public MainWindowViewModel()
        {
            _tabs.Connect()
                .Select(t => new TabViewModel())
        }
        
        public ValueTask DisposeAsync()
        {
            _tabs.Clear();
            return _modelScope.DisposeAsync();
        }
    }
}