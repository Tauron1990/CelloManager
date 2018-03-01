namespace Tauron.Application.CelloManager.Logic.Historie.DTO
{
    public class GetPageItemsData
    {
        public int Page { get; }

        public GetPageItemsData(int page)
        {
            Page = page;
        }
    }
}