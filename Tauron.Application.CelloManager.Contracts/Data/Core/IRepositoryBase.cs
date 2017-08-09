namespace Tauron.Application.CelloManager.Data.Core
{
    public interface IRepositoryBase
    {
        IDatabaseManager Manager { get; }
    }
}