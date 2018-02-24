using System.Linq;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.CelloManager.Data.Manager
{
    public sealed class SpoolRepository : Repository<CelloSpoolEntity, int>, ISpoolRepository
    {
        public SpoolRepository(IDatabase database) : base(database)
        {
        }
    }
}