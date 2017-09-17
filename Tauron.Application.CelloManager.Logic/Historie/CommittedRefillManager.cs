using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Logic.Historie
{
    [Export(typeof(ICommittedRefillManager))]
    public sealed class CommittedRefillManager : ICommittedRefillManager
    {
        [Inject]
        public ICommittedRefillRepository CommittedRefillRepository { private get; set; }

        [Inject]
        public IManagerEnviroment Enviroment { private get; set; }


        public IEnumerable<CommittedRefill> CommitedRefills => CommittedRefillRepository.GetCommittedRefills().OrderBy(val => val.SentTime);

        public void Purge()
        {
            using (CommittedRefillRepository.Manager.StartOperation())
            {
                var maxamount = Enviroment.Settings.MaximumSpoolHistorie;
                var count = CommittedRefillRepository.GetCommittedRefills().Count();

                if (count < maxamount) return;

                foreach (var purge in CommittedRefillRepository.GetCommittedRefills().OrderBy(r => r.SentTime).Take(count - maxamount))
                {
                    CommittedRefillRepository.Delete(purge);
                }
            }
        }
    }
}