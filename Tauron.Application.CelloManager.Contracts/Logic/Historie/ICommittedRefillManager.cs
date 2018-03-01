using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.CelloManager.Logic.Manager;

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

        bool IsRefillNeeded(IEnumerable<CelloSpool> spools, IEnumerable<CommittedRefill> orders);

        int GetPageCount();

        IEnumerable<CommittedRefill> GetPage(int pageCount);
    }
}