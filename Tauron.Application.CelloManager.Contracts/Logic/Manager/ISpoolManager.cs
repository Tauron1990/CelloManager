using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.CelloManager.Logic.Manager
{
    public interface ISpoolManager
    {
        [ItemNotNull]
        [NotNull]
        IEnumerable<CelloSpool> CelloSpools { get; }

        bool SpoolEmpty([NotNull] CelloSpool spool, int amount);

        CelloSpool AddSpool(CelloSpool spool);

        void AddSpoolAmount(CelloSpool spool, int amount);

        void UpdateSpools(IEnumerable<CelloSpool> spools);

        void RemoveSpool(CelloSpool spool);
    }
}