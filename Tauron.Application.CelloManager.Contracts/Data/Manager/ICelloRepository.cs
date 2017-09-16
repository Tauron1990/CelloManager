using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Logic.Manager;

namespace Tauron.Application.CelloManager.Data.Manager
{
    public interface ICelloRepository : IRepositoryBase
    {

        //bool Add([NotNull] string name, [NotNull] string type, int amount, int neededamount, out CelloSpoolBase spool);
        CelloSpoolBase Add();

        void Remove([NotNull] CelloSpoolBase entry);
        
        //void Update(CelloSpoolBase entry);
        IEnumerable<CelloSpoolBase> GetSpools();
    }
}