namespace Tauron.Application.CelloManager.Logic.Manager.DTO
{
    public class RemoveAmountResult
    {
        public bool Ok { get; }

        public RemoveAmountResult(bool ok)
        {
            Ok = ok;
        }
    }
}