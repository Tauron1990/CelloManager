using NUnit.Framework;

namespace Tauron.Application.Common.Updater.Tests
{
    [TestFixture]
    public class UpdaterServiceTests
    {
        [OneTimeSetUp]
        public void Initialize()
        {
            Impl.DebuggerService.StartDebug();
        }

        [Test]
        public void CommonTest()
        {
            Assert.Fail();
        }
    }
}