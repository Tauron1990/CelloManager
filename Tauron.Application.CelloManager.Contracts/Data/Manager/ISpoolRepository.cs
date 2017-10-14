using System.Linq;
using JetBrains.Annotations;

namespace Tauron.Application.CelloManager.Data.Manager
{
    public interface ISpoolRepository
    {
        //bool Add([NotNull] string name, [NotNull] string type, int amount, int neededamount, out CelloSpoolBase spool);
        CelloSpoolEntry Add();

        void Remove(int entry);
        
        //void Update(CelloSpoolBase entry);
        IQueryable<CelloSpoolEntry> GetSpools();

        CelloSpoolEntry GetEntry(int entry);
    }
}