using System;
using System.Linq;
using NUnit.Framework;
using Tauron.Application.Ioc;
using TestHelpers;

// ReSharper disable once CheckNamespace
namespace Tauron.Application.CelloManager.Data.Core.Tests
{
    [TestFixture, NonParallelizable, Order(Int32.MinValue)]
    public class UnitOfWorkTests
    {
        public static IUnitOfWorkFactory CreateFactory(params Type[] types)
        {
            IContainer container = new DefaultContainer();
            ExportResolver resolver = new ExportResolver();
            resolver.AddTypes(types.Concat(new[] {typeof(UnitOfWork)}));
            container.Register(resolver);
            return new UnitOfWorkFactory { Container = container };
        }

        private IUnitOfWork _unitOfWork;
        private IUnitOfWorkFactory _factory;

        [OneTimeSetUp]
        public void Initialize()
        {
            InitializeHelper.Initialize();

            CoreDatabase.OverrideConnection(DBHelper.DbConstring);
            using (var db = new CoreDatabase())
                db.UpdateSchema();

            _factory = CreateFactory(typeof(TestHelpers.Mocks.SpoolRepositoryMock));
        }

        [Test, Order(1)]
        public void UnitOfWorkTest()
        {
            _unitOfWork = _factory.CreateUnitOfWork();
        }

        [Test, Order(2)]
        public void CommitTest()
        {
            var repo = _unitOfWork.SpoolRepository;
            var ele = repo.Add();
            ele.Name = "Test";

            _unitOfWork.Commit();
        }

        [Test, Order(3)]
        public void DisposeTest()
        {
            _unitOfWork.Dispose();
        }
    }
}