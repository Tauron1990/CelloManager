using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Data.Core.Tests;
using Tauron.Application.CelloManager.Logic.Manager;
using TestHelpers;

// ReSharper disable once CheckNamespace
namespace Tauron.Application.CelloManager.Data.Manager.Tests
{
    [TestFixture, Order(1), NonParallelizable]
    public class CelloRepositoryTests
    {
        private static DataShuffler _dataShuffler;
        private IOperationManager _operationManager;

        [OneTimeSetUp]
        public void Initialize()
        {
            //if(!InitializeHelper.IsInitialized)
            //    Batteries_V2.Init();
            InitializeHelper.Initialize();

            _dataShuffler = new DataShuffler();

            CoreDatabase.OverrideConnection(DBHelper.DbConstring);
            using (CoreDatabase db = new CoreDatabase())
                db.UpdateSchema();

            _operationManager = new OperationManager();
        }

        [Test, Order(2)]
        public void GetSpoolsTest()
        {

            _operationManager.Enter(o =>
            {
                var num = o.Spools.GetSpools().Count();

                Assert.AreNotSame(0, num);
            });
        }

        [Test, Order(0)]
        public void AddTest()
        {
            int tempid = 0;

            _operationManager.Enter(o =>
            {
                var celloSpool = o.Spools.Add();
                Fill(celloSpool);
                o.Commit();

                tempid = o.Spools.GetSpools().First().Id;
            });

            int tempcount = 0;

            _operationManager.Enter(o =>
            {
                var rep = o.Spools;
                var data = rep.GetSpools().ToArray();
                tempcount = data.Length;

                Assert.NotNull(data.FirstOrDefault(r => r.Id == tempid));
            });

            _operationManager.Enter(o =>
            {
                var rep = o.Spools;
                rep.Add();
            });

            _operationManager.Enter(o =>
            {
                CelloSpoolEntry[] data2 = o.Spools.GetSpools().ToArray();

                Assert.AreEqual(tempcount, data2.Length);
            });
        }

        [Test, Order(3)]
        public void RemoveTest()
        {
            int count = 0;
            List<CelloSpoolEntry> data = new List<CelloSpoolEntry>();

            _operationManager.Enter(o =>
            {
                data = o.Spools.GetSpools().ToList();
                count = data.Count;
            });

            _operationManager.Enter(o =>
            {
                o.Spools.Remove(data[0].Id);
            });

            _operationManager.Enter(o =>
            {
                var data2 = o.Spools.GetSpools().ToArray();
                Assert.AreEqual(count, data2.Length);

                o.Spools.Remove(data[0].Id);

                o.Commit();
            });

            _operationManager.Enter(o =>
            {
                var data2 = o.Spools.GetSpools().ToArray();
                Assert.AreNotEqual(count, data2.Length);
            });
        }
        
        private static void Fill(CelloSpoolEntry spool)
        {
            spool.Neededamount = _dataShuffler.GetNumber();
            spool.Amount = _dataShuffler.GetNumber(spool.Neededamount);
            spool.Name = _dataShuffler.GetName();
            spool.Type = _dataShuffler.GetType();
        }
    }
}