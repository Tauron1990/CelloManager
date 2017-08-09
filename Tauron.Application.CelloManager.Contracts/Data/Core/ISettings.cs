using JetBrains.Annotations;

namespace Tauron.Application.CelloManager.Data.Core
{
    public interface ISettings
    {
        [CanBeNull]
        string DefaultPrinter { get; set; }

        int MaximumSpoolHistorie { get; set; }
    }
}