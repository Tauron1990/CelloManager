using System;
using System.Drawing.Printing;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ReactiveUI;

namespace CelloManager.ViewModels.Printing;

public sealed class PrinterSettingModel : ReactiveObject
{
    public PrinterSettings Settings { get; set; } = new();

    public string SelectedPrinter
    {
        get => Settings.PrinterName;
        [UsedImplicitly]
        set => UpdateSettings(value, (settings, s) => settings.PrinterName = s);
    }

    private void UpdateSettings<TValue>(TValue value, Action<PrinterSettings, TValue> update, [CallerMemberName] string? name = default)
    {
        update(Settings, value);
        this.RaisePropertyChanged(name);
    }
}