using System;
using System.Drawing.Printing;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CelloManager.Core.Data;
using CelloManager.Core.Printing;
using DynamicData;
using DynamicData.Binding;
using JetBrains.Annotations;
using ReactiveUI;

namespace CelloManager.ViewModels.Printing;

public sealed class PrintDialogViewModel : ViewModelBase, IDisposable
{
    private readonly PrintProgressManager _printManager;
    private readonly ErrorDispatcher _errorDispatcher;
    private readonly PreviewManagerModel _previewManager = new();
    
    private readonly BehaviorSubject<bool> _isValid = new(false);
    private readonly CompositeDisposable _compositeDisposable = new();
    
    private string _selectedPrinter = string.Empty;

    public PrinterSettingModel PrinterSettingModel { get; } = new();
    
    public IObservableList<string> Printers { get; }
    
    public ReactiveCommand<Unit, Unit> RefreshPrinters { get; }

    public PrintDialogViewModel(PrintProgressManager printManager, ErrorDispatcher errorDispatcher)
    {
        _printManager = printManager;
        _errorDispatcher = errorDispatcher;

        printManager.InstalledPrinters
            .ObserveOn(RxApp.MainThreadScheduler)
            .BindToObservableList(out var printerList)
            .Subscribe()
            .DisposeWith(_compositeDisposable);

        Printers = printerList;
        
        RefreshPrinters = ReactiveCommand.CreateFromTask(() => Task.Run(_printManager.RefreshPrinters), CanClick());
    }

    private IObservable<bool> CanClick()
    {
        return _printManager
            .WhenAnyValue(m => m.IsInProgress)
            .CombineLatest(
                _isValid,
                _printManager.WhenAnyValue(m => m.PrintDocument).Select(pd => pd != null),
                (progress, docOk, valid) => !progress && valid && docOk);
    }

    public void Init(PrintDocument document)
        => Task.Run(() => InitInternal(document));

    private void InitInternal(PrintDocument document)
    {
        try
        {
            _printManager.Init(document);

            PrinterSettingModel.Settings = document.PrinterSettings;
            PrinterSettingModel.SelectedPrinter = document.PrinterSettings.PrinterName;
            
            _previewManager.MakePreView(document);
            
            _isValid.OnNext(true);
        }
        catch (Exception e)
        {
            _errorDispatcher.Send(e);
            _isValid.OnNext(false);
        }
    }

    public void Dispose()
    {
        _isValid.Dispose();
        _printManager.Dispose();
        _compositeDisposable.Dispose();
        _previewManager.Dispose();
    }
}