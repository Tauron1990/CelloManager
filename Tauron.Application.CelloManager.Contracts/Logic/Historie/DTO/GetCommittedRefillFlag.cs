namespace Tauron.Application.CelloManager.Logic.Historie.DTO
{
    public sealed class GetCommittedRefillFlag
    {
        public GetCommittedRefillFlag(bool getCompleted)
        {
            GetCompleted = getCompleted;
        }

        public bool GetCompleted { get; }
    }
}