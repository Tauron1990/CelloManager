using Tauron.Application.CelloManager.Data.Historie;

namespace Tauron.Application.CelloManager.Logic.Manager
{
    public sealed class PrintOrderEventArgs
    {
        public CommittedRefill Refill { get; }

        public bool Ok { get; set; }

        public PrintOrderEventArgs(CommittedRefill refill)
        {
            Refill = refill;

            Ok = false;
        }
    }
}