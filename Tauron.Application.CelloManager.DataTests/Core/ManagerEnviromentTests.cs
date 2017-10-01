using NUnit.Framework;
using TestHelpers.Mocks;

// ReSharper disable once CheckNamespace
namespace Tauron.Application.CelloManager.Data.Core.Tests
{
    [TestFixture, NonParallelizable, Order(1)]
    public class ManagerEnviromentTests
    {
        private ManagerEnviroment _testSubject;

        [OneTimeSetUp]
        public void Initialize()
        {
            _testSubject = new ManagerEnviroment { UnitOfWorkFactory = new UnitOfWorkFactoryMock { OptionsRepository = new OptionsRepositoryMock() } };
        }

        [Test, Order(1)]
        public void ManagerEnviromentTest()
        {
            _testSubject.Settings.DefaultPrinter = "Test";

            Assert.AreEqual(_testSubject.Settings.DefaultPrinter, "Test");

            _testSubject.Settings.MaximumSpoolHistorie = 5;

            Assert.AreEqual(_testSubject.Settings.MaximumSpoolHistorie, 5);
        }

        [Test, Order(2)]
        public void SaveTest()
        {
            _testSubject.Save();
        }

        [Test, Order(3)]
        public void BuildCompledTest()
        {
            _testSubject.BuildCompled();
            Assert.AreEqual(_testSubject.Settings.DefaultPrinter, "Test");
        }
    }
}