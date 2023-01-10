using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using CelloManager.ViewModels.Printing;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace CelloManager.Views.Printing;

public partial class PrintingDialog : ReactiveWindow<PrintDialogViewModel>
{
    [UsedImplicitly]
    public PrintingDialog()
    {
        InitializeComponent();
        
        Closed += OnClosed;

        this.WhenActivated(Init);
    }

    private IEnumerable<IDisposable> Init()
    {
        if(ViewModel is null) yield break;

        yield return this.OneWayBind(ViewModel, m => m.Printers, v => v.AvailablePrinters.Items);
        yield return this.Bind(ViewModel, m => m.PrinterSettingModel.SelectedPrinter, v => v.AvailablePrinters.SelectedItem);
        
        yield return this.BindCommand(ViewModel, m => m.RefreshPrinters, v => v.RefreshAvailablePrintersCommand);
    }

    private void OnClosed(object? sender, EventArgs e) => ViewModel?.Dispose();

    public static async ValueTask<bool> ShowAsync(PrintDocument document, Dispatcher dispatcher, IServiceProvider serviceProvider)
    {
        var model = serviceProvider.GetRequiredService<PrintDialogViewModel>();
        model.Init(document);
        
        return await dispatcher.InvokeAsync(
            async () =>
            {
                var window = new PrintingDialog
                {
                    ViewModel = model,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                };
                
                return await window.ShowDialog<bool>(App.MainWindow);
            });
    }
}