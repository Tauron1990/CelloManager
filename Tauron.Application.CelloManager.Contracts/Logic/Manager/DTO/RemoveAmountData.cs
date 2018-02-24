namespace Tauron.Application.CelloManager.Logic.Manager.DTO
{
    public class RemoveAmountData
    {
        public CelloSpool CelloSpool { get; }

        public int Amount { get; }

        public RemoveAmountData(CelloSpool celloSpool, int amount)
        {
            CelloSpool = celloSpool;
            Amount     = amount;
        }
    }
}