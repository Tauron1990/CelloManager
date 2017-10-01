using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Data;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Logic.Historie
{
    [Export(typeof(ICommittedRefillManager))]
    public sealed class CommittedRefillManager : ICommittedRefillManager
    {
        [Inject]
        public IUnitOfWorkFactory UnitOfWorkFactory { private get; set; }

        [Inject]
        public IManagerEnviroment Enviroment { private get; set; }


        public IEnumerable<CommittedRefill> CommitedRefills
        {
            get
            {
                using (var work = UnitOfWorkFactory.CreateUnitOfWork())
                    return work.CommittedRefillRepository.GetCommittedRefills().OrderBy(val => val.SentTime).ToArray();
            }
        }

        public void Purge()
        {
            using (var work = UnitOfWorkFactory.CreateUnitOfWork())
            {
                var maxamount = Enviroment.Settings.MaximumSpoolHistorie;
                var count = work.CommittedRefillRepository.GetCommittedRefills().Count();

                if (count < maxamount) return;

                foreach (var purge in work.CommittedRefillRepository.GetCommittedRefills().OrderBy(r => r.SentTime).Take(count - maxamount))
                {
                    work.CommittedRefillRepository.Delete(purge);
                }
            }
        }
    }
}