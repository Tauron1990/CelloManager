using Tauron.Application.CelloManager.Logic.Historie;

namespace Tauron.Application.CelloManager.Logic.RefillPrinter
{
    public interface IRefillPrinter
    {
        bool Print(CommittedRefill refill);
    }
}