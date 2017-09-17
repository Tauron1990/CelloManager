using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Logic.Manager
{
    [Export(typeof(ISpoolManager))]
    public sealed class SpoolManager : ISpoolManager
    {
        [Inject]
        public IManagerEnviroment Enviroment { private get; set; }

        [Inject]
        public ICommittedRefillRepository RefillRepository { private get; set; }

        [Inject]
        public ICelloRepository Spools { private get; set; }

        public IEnumerable<CelloSpoolBase> CelloSpools => Spools.GetSpools();


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

        public void UpdateSpools(IEnumerable<Action> updater)
        {
            using (Spools.Manager.StartOperation())
                foreach (var action in updater)
                    action();
        }

        public void AddSpool(CelloSpoolBase spool, int value)
        {
            spool.Amount += value;
        }

        private void CommonOrder(Action<PrintOrderEventArgs> spectialAction)
        {
            CommittedRefill refill;

            using (Spools.Manager.StartOperation())
            {
                refill = ReadSpools();
                var args = new PrintOrderEventArgs(refill);
                spectialAction(args);

                if (!args.Ok) return;

                InsertInDatabase(refill);
                ResetSpools(refill);
                Spools.Manager.SaveChanges = true;
            }

            EventAggregator.Aggregator.GetEvent<OrderSentEvent, CommittedRefill>().Publish(refill);
        }

        private void InsertInDatabase(CommittedRefill refill)
        {
            RefillRepository.Add(refill);
        }

        private void ResetSpools(CommittedRefill refill)
        {
            Enviroment.Save();

            foreach (var entry in refill.CommitedSpools.Select(spool => CelloSpools.First(s => s.Name == spool.Name)))
                entry.Amount = entry.Neededamount;
        }

        private CommittedRefill ReadSpools()
        {
            var spools = from celloSpool in CelloSpools
                where celloSpool.Amount != celloSpool.Neededamount
                let diff = celloSpool.Neededamount - celloSpool.Amount
                where diff > 0
                select new CommittedSpool(celloSpool.Name, diff, celloSpool.Type);

            return new CommittedRefill { CommitedSpools = spools.ToList(), SentTime = DateTime.Now};
        }
    }
}