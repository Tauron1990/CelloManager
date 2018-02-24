using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.CelloManager.Logic.Historie
{
    public interface ICommittedRefillManager
    {
        [ItemNotNull]
        [NotNull]
        IEnumerable<CommittedRefill> CommitedRefills { get; }

        IEnumerable<CommittedRefill> PlacedOrders { get; }

        void Purge();
        
        CommittedRefill PlaceOrder();

        void CompledRefill(CommittedRefill refill);

        bool IsRefillNeeded();
    }
}