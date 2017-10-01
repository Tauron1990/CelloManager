using Tauron.Application.CelloManager.Data.Core;

namespace Tauron.Application.CelloManager.Data
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateUnitOfWork();
    }
}