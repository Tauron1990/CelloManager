using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.CelloManager.Data.Core;

namespace Tauron.Application.CelloManager.Logic.Manager
{
    public interface ISpoolManager
    {
        [ItemNotNull]
        [NotNull]
        IEnumerable<CelloSpoolBase> CelloSpools { get; }

        void SpoolEmty([NotNull] CelloSpoolBase spool);

        void AddSpool(CelloSpoolBase spool, int value);

        void PrintOrder();

        bool IsRefillNeeded();

        void UpdateSpools(IEnumerable<Action<IUnitOfWork>> updater);
    }
}