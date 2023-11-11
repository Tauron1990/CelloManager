using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CelloManager.Core.Data;
using CelloManager.Core.Logic;
using CelloManager.Core.Printing;
using CelloManager.ViewModels.Editing;
using CelloManager.ViewModels.Importing;
using CelloManager.ViewModels.Orders;
using CelloManager.ViewModels.SpoolDisplay;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
using JetBrains.Annotations;
using ReactiveUI;
using ThingLing.Controls;

namespace CelloManager.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IAsyncDisposable
    {
        private readonly AppServiceProvider.Scope _modelScope = App.ServiceProvider.CreateScope();
        private readonly SourceList<ViewModelBase> _tabs = new();
        private readonly CompositeDisposable _subscriptions = new();
        private readonly ObservableAsPropertyHelper<string> _errorSimple;
        private readonly ObservableAsPropertyHelper<string> _errorFull;
        
        private readonly SpoolManager _spoolManager;
        private readonly ErrorDispatcher _errors;

        private int _currentTab;

        public string ErrorSimple => _errorSimple.Value;

        public string ErrorFull => _errorFull.Value;

        public int CurrentTab
        {
            get => _currentTab;
            [UsedImplicitly]
            set => this.RaiseAndSetIfChanged(ref _currentTab, value);
        }

        private readonly ObservableAsPropertyHelper<string> _priceValue;
        [UsedImplicitly]
        public string PriceValue => _priceValue.Value;
        
        public IObservableCollection<TabViewModel> Tabs { get; } = new ObservableCollectionExtended<TabViewModel>();

        public ReactiveCommand<Unit, Unit> Edit { get; }
        
        public ReactiveCommand<Unit, Unit> Import { get; }
        
        public ReactiveCommand<Unit, Unit> Order { get; }
        
        public ReactiveCommand<Unit, Unit> Orders { get; }

        public ReactiveCommand<Unit, Unit> PrintAll { get; }

        public ReactiveCommand<Unit, Unit> Export { get; }

        public void DisplayTab<TTab>()
            where TTab : ViewModelBase
        {
            int index = -1;

            for (int i = 0; i < _tabs.Count; i++)
            {
                if(_tabs.Items.ElementAt(i) is not TTab)
                    continue;

                index = i;
                break;
            }

            if(index == -1)
                _tabs.Add(_modelScope.GetService<TTab>());
            else
                CurrentTab = index;
        }
        
        #pragma warning disable MA0051
        public MainWindowViewModel(ErrorDispatcher errors)
            #pragma warning restore MA0051
        {
            _errors = errors;
            var priceManager = _modelScope.GetService<SpoolPriceManager>();
            _spoolManager = _modelScope.GetService<SpoolManager>();
            var dispatcher = Dispatcher.UIThread;

            _priceValue = priceManager.CompledPrice
                .StartWith(0)
                .Select(p => p > 0 ? $"{p:N} Euro" : "")
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, m => m.PriceValue);
            
            var currentTabs = _tabs.Connect().QueryWhenChanged().Publish().RefCount();
            var orderManager = _modelScope.GetService<OrderManager>();
            var builder = _modelScope.GetService<PrintBuilder>();

            Export = ReactiveCommand.CreateFromTask(ExportToJson, _spoolManager.Count.Select(i => i != 0));
            
            PrintAll = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    try
                    {
                        await builder.PrintPendingOrder(orderManager.GetAll(), dispatcher, App.ServiceProvider, null).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        errors.Send(e);
                    }
                });
            
            Edit = ReactiveCommand.Create
                (
                    () => _tabs.Add(_modelScope.GetService<EditTabViewModel>()),
                    ContainsViewModel<EditTabViewModel>(currentTabs)
                )
                .DisposeWith(_subscriptions);
            
            Orders = ReactiveCommand.Create(
                    DisplayTab<OrderDisplayViewModel>,
                    ContainsViewModel<OrderDisplayViewModel>(currentTabs))
                .DisposeWith(_subscriptions);
            
            Import = ReactiveCommand.Create(
                () => _tabs.Add(_modelScope.GetService<ImportViewModel>()),
                ContainsViewModel<ImportViewModel>(currentTabs)).DisposeWith(_subscriptions);
            
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
            
            _subscriptions.Add(errors.Errors.SelectMany(
                ex => dispatcher.InvokeAsync(
                    async () =>
                    {

                        await MessageBox.ShowAsync(App.MainWindow, ex.Message, "Fehler", MessageBoxButton.Ok).ConfigureAwait(false);
                        return Unit.Default;
                    }))
                .Subscribe());
        }

        private async Task ExportToJson()
        {
            try
            {
                var file = await App.StorageProvider
                                    .SaveFilePickerAsync(
                                         new FilePickerSaveOptions
                                         {
                                             Title = "Daten Exportieren",
                                             SuggestedFileName = "Spulen.json",
                                             DefaultExtension = "json",
                                             FileTypeChoices = new[]
                                                               {
                                                                   new FilePickerFileType("Json Daten")
                                                                   {
                                                                       Patterns = new[] { "json" },
                                                                   },
                                                               },
                                         })
                                    .ConfigureAwait(false);
                
                if(file is null) return;

                var error = await _spoolManager.ExportToJson(file).ConfigureAwait(false);
                
                if(error is null) return;
                
                _errors.Send(error);
            }
            catch (Exception e)
            {
                _errors.Send(e);
            }
        }

        private IObservable<bool> ContainsViewModel<TModel>(IObservable<IReadOnlyCollection<ViewModelBase>> query)
            => query.Select(l => l.All(vm => vm.GetType() != typeof(TModel))).ObserveOn(RxApp.MainThreadScheduler);

        public ValueTask DisposeAsync()
        {
            _priceValue.Dispose();
            _subscriptions.Dispose();
            _tabs.Clear();
            _tabs.Dispose();
            GC.SuppressFinalize(this);
            return _modelScope.DisposeAsync();
        }
    }
}