using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Data.Manager
{
    [Export(typeof(ICelloRepository))]
    public sealed class CelloRepository : RepositoryBase, ICelloRepository
    {
        public bool Add(string name, string type, int amount, int neededamount, out CelloSpoolBase spool)
        {
            using (CoreManager.StartOperation())
            {
                var uname = CelloSpoolBase.BuildUinqueId(name, type);
                var temp = CoreManager.Database.CelloSpools.ToList().FirstOrDefault(e => CelloSpool.BuildUinqueId(e) == uname);
                if (temp != null)
                {
                    spool = new CelloSpool(temp, this);
                    return false;
                }

                temp = new CelloSpoolEntry {Name = name, Type = type, Amount = amount, Neededamount = neededamount, Timestamp = DateTime.Now};

                CoreManager.Database.CelloSpools.Add(temp);
                spool = new CelloSpool(temp, this, true);
            }

            return true;
        }

        public void Remove(CelloSpoolBase entry)
        {
            using (CoreManager.StartOperation())
                CoreManager.Database.CelloSpools.Remove(((CelloSpool) entry).CoreEntry);
        }

        public IEnumerable<CelloSpoolBase> GetSpools()
        {
            using (CoreManager.StartOperation())
            {
                CoreManager.SaveChanges = false;
                foreach (var spool in CoreManager.Database.CelloSpools)
                {
                    yield return new CelloSpool(spool, this);
                }
            }
        }

        internal void UpdateEntry(CelloSpoolEntry entry)
        {
            using (CoreManager.StartOperation())
                CoreManager.Database.Update(entry);
        }

        public CelloSpoolEntry GetEntry(string name, string type)
        {
            using (CoreManager.StartOperation())
                return CoreManager.Database.CelloSpools.Single(c => c.Name == name && c.Type == type);
        }
    }
}