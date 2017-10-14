using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.CelloManager.Data.Historie;

namespace Tauron.Application.CelloManager.Logic.Historie
{
    public interface ICommittedRefillManager
    {
        [ItemNotNull]
        [NotNull]
        IEnumerable<CommittedRefill> CommitedRefills { get; }

        void Purge();
        
        void Refill();

        bool IsRefillNeeded();
    }
}