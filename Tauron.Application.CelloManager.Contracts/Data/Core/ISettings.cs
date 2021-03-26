using JetBrains.Annotations;
using Tauron.Application.CelloManager.Logic.RefillPrinter;

namespace Tauron.Application.CelloManager.Data.Core
{
    public interface ISettings
    {
        string Dns { get; set; }

        string TargetEmail { get; set; }

        RefillPrinterType PrinterType { get; set; }

        bool Purge { get; set; }

        int Threshold { get; set; }

        [CanBeNull]
        string DefaultPrinter { get; set; }

        int MaximumSpoolHistorie { get; set; }

        string DockingState { get; set; }

        string SpoolDataGridState { get; set; }

        long EmailPort { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string Server { get; set; }
        bool DomainMode { get; set; }
        string Domain { get; set; }
    }
}