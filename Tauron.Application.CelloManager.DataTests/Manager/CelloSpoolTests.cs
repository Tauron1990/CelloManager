using NUnit.Framework;
using Tauron.Application.CelloManager.Data.Core;
using TestHelpers;

// ReSharper disable once CheckNamespace
namespace Tauron.Application.CelloManager.Data.Manager.Tests
{
    [TestFixture, Order(2)]
    public class CelloSpoolTests
    {
        private static CelloSpool _testSpool;

        [OneTimeSetUp]
        public void Initialize()
        {
            _testSpool = new CelloSpool(new CelloSpoolEntry());
        }

        [Test, Order(0)]
        public void CelloSpoolTest()
        {
            DataShuffler dataShuffler = new DataShuffler();

            Assert.IsTrue(_testSpool.HasErrors);
            _testSpool.Name = dataShuffler.GetName();
            _testSpool.Type = dataShuffler.GetType();
            Assert.IsTrue(_testSpool.HasNoErrors);
            _testSpool.Neededamount = -5;
            _testSpool.Amount = -5;
            Assert.IsTrue(_testSpool.HasErrors);

            Assert.AreEqual(_testSpool.GetIssuesDictionary().AllValues.Count, 2);
        }
    }
}