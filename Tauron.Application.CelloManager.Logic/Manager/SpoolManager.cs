using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Data;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Logic.Manager
{
    [Export(typeof(ISpoolManager))]
    public sealed class SpoolManager : ISpoolManager
    {
        [Inject]
        public IManagerEnviroment Enviroment { private get; set; }

        [Inject]
        public IUnitOfWorkFactory UnitOfWorkFactory { get; set; }
        

        public IEnumerable<CelloSpoolBase> CelloSpools
        {
            get
            {
                using (var work = UnitOfWorkFactory.CreateUnitOfWork())
                    return work.SpoolRepository.GetSpools();
            }
        }


        public void SpoolEmty(CelloSpoolBase spool)
        {
            if (spool.Amount == 0)
                return;

            spool.Amount -= 1;
        }

        public void PrintOrder()
        {
            CommonOrder(c => EventAggregator.Aggregator.GetEvent<PrintOrderEvent, PrintOrderEventArgs>().Publish(c));
        }

        public bool IsRefillNeeded()
        {
            return CelloSpools.Any(s => s.Neededamount > s.Amount);
        }

        public void UpdateSpools(IEnumerable<Action<IUnitOfWork>> updater)
        {
            using (var work = UnitOfWorkFactory.CreateUnitOfWork())
                foreach (var action in updater)
                    action(work);
        }

        public void AddSpool(CelloSpoolBase spool, int value)
        {
            spool.Amount += value;
        }

        private void CommonOrder(Action<PrintOrderEventArgs> spectialAction)
        {
            CommittedRefill refill;

            using (var work = UnitOfWorkFactory.CreateUnitOfWork())
            {
                var spools = work.SpoolRepository.GetSpools().ToArray();
                refill = ReadSpools(spools);
                var args = new PrintOrderEventArgs(refill);
                spectialAction(args);

                if (!args.Ok) return;

                work.CommittedRefillRepository.Add(refill);
                ResetSpools(refill, spools);
                
                work.Commit();
            }

            EventAggregator.Aggregator.GetEvent<OrderSentEvent, CommittedRefill>().Publish(refill);
        }

        private void ResetSpools(CommittedRefill refill, IEnumerable<CelloSpoolBase> celloSpolls)
        {
            //Enviroment.Save();

            foreach (var entry in refill.CommitedSpools.Select(spool => celloSpolls.First(s => s.Id == spool.SpoolId)))
                entry.Amount = entry.Neededamount;
        }

        private CommittedRefill ReadSpools(IEnumerable<CelloSpoolBase> celloSpolls)
        {
            var spools = from celloSpool in celloSpolls
                where celloSpool.Amount != celloSpool.Neededamount
                let diff = celloSpool.Neededamount - celloSpool.Amount
                where diff > 0
                select new CommittedSpool(celloSpool.Name, diff, celloSpool.Type, celloSpool.Id);

            return new CommittedRefill {CommitedSpools = spools.ToList(), SentTime = DateTime.Now};
        }
    }
}