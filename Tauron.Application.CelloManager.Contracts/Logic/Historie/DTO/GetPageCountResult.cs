namespace Tauron.Application.CelloManager.Logic.Historie.DTO
{
    public sealed class GetPageCountResult
    {
        public int Count { get; }

        public GetPageCountResult(int count)
        {
            Count = count;
        }
    }
}