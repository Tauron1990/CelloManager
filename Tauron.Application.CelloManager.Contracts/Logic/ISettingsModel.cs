using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using Tauron.Application.CelloManager.Logic.RefillPrinter;

namespace Tauron.Application.CelloManager.Logic
{
    public interface ISettingsModel : INotifyDataErrorInfo
    {
        void Save();
        bool CanSave();
        void Cancel();
        string ErrorText { get; }
        [UsedImplicitly]
        string Dns { get; set; }
        [UsedImplicitly]
        string TargetEmail { get; set; }
        [UsedImplicitly]
        RefillPrinterType PrinterType { get; set; }
        [UsedImplicitly]
        Dictionary<RefillPrinterType, string> PrinterTypeCaptions { get; }
        [UsedImplicitly]
        bool Purge { get; set; }
        [UsedImplicitly]
        int Threshold { get; set; }
        [UsedImplicitly]
        string DefaultPrinter { get; set; }
        [UsedImplicitly]
        int MaximumSpoolHistorie { get; set; }
    }
}