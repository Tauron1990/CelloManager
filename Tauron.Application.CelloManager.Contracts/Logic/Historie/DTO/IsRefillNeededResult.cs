namespace Tauron.Application.CelloManager.Logic.Historie.DTO
{
    public class IsRefillNeededResult
    {
        public IsRefillNeededResult(bool need)
        {
            Need = need;
        }

        public bool Need { get; }
    }
}