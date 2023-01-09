using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.Threading;
using DynamicData.Binding;
using ReactiveUI;
using static System.Drawing.Printing.PrinterSettings;

namespace CelloManager.Core.Movere.ViewModels
{
    public sealed class PrintDialogViewModel : ReactiveObject
    {
        private readonly PrintDocument _document;
        private readonly PreviewPrintController _controller = new();

        private readonly Action<bool> _closeAction;

        private IReadOnlyList<PrintPreviewPageViewModel> _printPreviewPages;

        private IReadOnlyList<string> _availablePrinters = InstalledPrinters.ToReadOnlyList();

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public PrintDialogViewModel(PrintDocument document, Action<bool> closeAction)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            _document = document;
            _closeAction = closeAction;

            _document.PrintController = _controller;

            PrinterSettings = new PrinterSettingsViewModel((PrinterSettings)_document.PrinterSettings.Clone());

            PrinterSettings
                .WhenAnyValue(vm => vm.PrinterSettings)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(UpdatePrintPreview);

            RefreshAvailablePrintersCommand = ReactiveCommand.Create(RefreshAvailablePrinters);

            PrintCommand = ReactiveCommand.Create(
                Print,
                PrinterSettings.WhenValueChanged(m => m.PrinterName).Select(x => !string.IsNullOrWhiteSpace(x)));
            
            CancelCommand = ReactiveCommand.Create(Cancel);

            RefreshAvailablePrinters();
        }

        public ICommand RefreshAvailablePrintersCommand { get; }

        public ICommand PrintCommand { get; }

        public ICommand CancelCommand { get; }

        public PrinterSettingsViewModel PrinterSettings { get; }

        public IReadOnlyList<string> AvailablePrinters
        {
            get => _availablePrinters;
            set => this.RaiseAndSetIfChanged(ref _availablePrinters, value);
        }

        public IReadOnlyList<PrintPreviewPageViewModel> PrintPreviewPages
        {
            get => _printPreviewPages;
            set => this.RaiseAndSetIfChanged(ref _printPreviewPages, value);
        }

        private void RefreshAvailablePrinters() => AvailablePrinters = InstalledPrinters.ToReadOnlyList();

        private void Print()
        {
            _document.PrinterSettings = PrinterSettings.PrinterSettings;
            Close(true);
        }

        private void Cancel() => Close(false);

        private void Close(bool result)
        {
            _document.PrintController = new StandardPrintController();
            _closeAction(result);
        }

        private void UpdatePrintPreview(PrinterSettings printerSettings)
        {
            _document.Print();

            var previewPageInfos = _controller.GetPreviewPageInfo();
            var printPreviewPages = new List<PrintPreviewPageViewModel>(previewPageInfos.Length);
            
            printPreviewPages.AddRange(previewPageInfos.Select(info => new PrintPreviewPageViewModel(info)));

            Dispatcher.UIThread.Post(() => PrintPreviewPages = printPreviewPages);
        }
    }
}
