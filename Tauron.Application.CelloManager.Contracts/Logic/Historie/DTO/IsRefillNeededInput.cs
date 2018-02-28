using Tauron.Application.CelloManager.Logic.Manager;

namespace Tauron.Application.CelloManager.Logic.Historie.DTO
{
    public class IsRefillNeededInput
    {
        public CelloSpool[] Spools { get; }

        public CommittedRefill[] Refills { get; }
        
        public IsRefillNeededInput(CelloSpool[] spools, CommittedRefill[] refills)
        {
            Spools = spools;
            Refills = refills;
        }
    }
}