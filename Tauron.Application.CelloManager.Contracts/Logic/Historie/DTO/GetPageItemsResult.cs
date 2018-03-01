using System.Collections.Generic;

namespace Tauron.Application.CelloManager.Logic.Historie.DTO
{
    public class GetPageItemsResult
    {
        public IEnumerable<CommittedRefill> CommittedRefill { get; }

        public GetPageItemsResult(IEnumerable<CommittedRefill> committedRefill)
        {
            CommittedRefill = committedRefill;
        }
    }
}