using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.CelloManager.Logic.Manager
{
    public interface ISpoolManager
    {
        [ItemNotNull]
        [NotNull]
        IEnumerable<CelloSpoolBase> CelloSpools { get; }

        void SpoolEmty([NotNull] CelloSpoolBase spool);

        void AddSpool(CelloSpoolBase spool, int value);
    }
}