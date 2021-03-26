using System.Collections.Generic;
using System.ComponentModel;
using Tauron.Application.CelloManager.Logic.RefillPrinter;

namespace Tauron.Application.CelloManager.Logic
{
    public interface ISettingsModel : INotifyDataErrorInfo
    {
        void Save();
        bool CanSave();
        void Cancel();
        string ErrorText { get; }
        string Dns { get; set; }
        string TargetEmail { get; set; }
        RefillPrinterType PrinterType { get; set; }
        Dictionary<RefillPrinterType, string> PrinterTypeCaptions { get; }
        bool Purge { get; set; }
        bool EmailServerMode { get; set; }
        int Threshold { get; set; }
        string DefaultPrinter { get; set; }
        int MaximumSpoolHistorie { get; set; }
        long? EmailPort { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string Server { get; set; }
        bool DomainMode { get; set; }
        string Domain { get; set; }
    }
}