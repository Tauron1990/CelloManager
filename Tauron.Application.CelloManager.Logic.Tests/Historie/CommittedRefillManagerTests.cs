using System.Linq;
using NUnit.Framework;
using TestHelpers.Mocks;

// ReSharper disable once CheckNamespace
namespace Tauron.Application.CelloManager.Logic.Historie.Tests
{
    [TestFixture]
    public class CommittedRefillManagerTests
    {
        [Test]
        public void PurgeTest()
        {
            var repmock = new CommittedSpoolRepositoryMock(10);
            ICommittedRefillManager manager = new CommittedRefillManager
            {
                Enviroment = new ManagerEnviromentMock(),
                UnitOfWorkFactory = new UnitOfWorkFactoryMock {  CommittedRefillRepository = repmock }
            };

            Assert.AreEqual(manager.CommitedRefills.Count(), 10);

            manager.Purge();

            Assert.AreEqual(manager.CommitedRefills.Count(), 5);
        }
    }
}