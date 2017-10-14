using Tauron.Application.CelloManager.Data.Historie;

namespace Tauron.Application.CelloManager.Logic.RefillPrinter
{
    public interface IRefillPrinter
    {
        void Print(CommittedRefill refill);
    }
}