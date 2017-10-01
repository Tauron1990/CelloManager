using System.Linq;
using NUnit.Framework;
using TestHelpers.Mocks;

// ReSharper disable once CheckNamespace
namespace Tauron.Application.CelloManager.Logic.Manager.Tests
{
    [TestFixture]
    public class SpoolManagerTests
    {
        private ISpoolManager _spoolManager;

        [OneTimeSetUp]
        public void Initialize()
        {
            var refillRepository = new CommittedSpoolRepositoryMock();
            var spools = new SpoolRepositoryMock(10);

            EventAggregator.Aggregator = new EventAggregator();
            EventAggregator.Aggregator.GetEvent<PrintOrderEvent, PrintOrderEventArgs>().Subscribe(a => a.Ok = true);

            _spoolManager = new SpoolManager
            {
                Enviroment = new ManagerEnviromentMock(),
                UnitOfWorkFactory = new UnitOfWorkFactoryMock { CommittedRefillRepository = refillRepository, SpoolRepository = spools }
            };
        }

        [Test, Order(1)]
        public void AddSpoolTest()
        {
            var celloSpoolBase = _spoolManager.CelloSpools.First();
            int baseValue = celloSpoolBase.Amount;

            _spoolManager.AddSpool(celloSpoolBase, 5);

            Assert.AreEqual(baseValue + 5, celloSpoolBase.Amount);

        }

        [Test, Order(2)]
        public void SpoolEmtyTest()
        {
            var celloSpoolBase = _spoolManager.CelloSpools.First();
            int baseValue = celloSpoolBase.Amount;

            _spoolManager.SpoolEmty(celloSpoolBase);

            Assert.AreEqual(baseValue - 1, celloSpoolBase.Amount);
        }

        [Test, Order(3)]
        public void IsRefillNeededTest()
        {
            var celloSpoolBase = _spoolManager.CelloSpools.First();
            celloSpoolBase.Amount = 0;
            celloSpoolBase.Neededamount = 5;

            Assert.IsTrue(_spoolManager.IsRefillNeeded());
        }

        [Test, Order(4)]
        public void PrintOrderTest()
        {
            var celloSpoolBase = _spoolManager.CelloSpools.First();
            celloSpoolBase.Amount = 0;
            celloSpoolBase.Neededamount = 5;

            _spoolManager.PrintOrder();

            foreach (var spool in _spoolManager.CelloSpools)
                Assert.AreEqual(spool.Neededamount, spool.Amount);
        }
    }
}