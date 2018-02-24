namespace Tauron.Application.CelloManager.Logic.Manager.DTO
{
    public class AddAmountData
    {
        public CelloSpool CelloSpool { get; }

        public int Amount { get; }

        public AddAmountData(CelloSpool celloSpool, int amount)
        {
            CelloSpool = celloSpool;
            Amount = amount;
        }
    }
}