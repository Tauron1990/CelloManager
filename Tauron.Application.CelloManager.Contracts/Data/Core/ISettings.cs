using JetBrains.Annotations;

namespace Tauron.Application.CelloManager.Data.Core
{
    public interface ISettings
    {
        bool Purge { get; set; }

        [CanBeNull]
        string DefaultPrinter { get; set; }

        int MaximumSpoolHistorie { get; set; }

        string DockingState { get; set; }

        string SpoolDataGridState { get; set; }
    }
}