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
        private IUnitOfWorkFactory _unitOfWorkFactory;

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

            _unitOfWorkFactory = UnitOfWorkTests.CreateFactory(typeof(SpoolRepository));
        }

        [Test, Order(1)]
        public void UpdateEntryTest()
        {
            using (var work = _unitOfWorkFactory.CreateUnitOfWork())
            {
                for (int i = 0; i < 4; i++)
                {
                    var temp = work.SpoolRepository.Add();
                    Fill(temp);
                    work.SpoolRepository.UpdateEntry(temp);
                }
                
                work.Commit();
            }
        }

        [Test, Order(2)]
        public void GetSpoolsTest()
        {

            using (var work = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var num = work.SpoolRepository.GetSpools().Count();

                Assert.AreNotSame(0, num);
            }
        }

        [Test, Order(0)]
        public void AddTest()
        {
            int tempid;

            using (var work = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var celloSpool = work.SpoolRepository.Add();
                Fill(celloSpool);
                work.Commit();

                tempid = work.SpoolRepository.GetSpools().First().Id;
            }

            int tempcount;

            using (var work = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var rep = work.SpoolRepository;
                var data = rep.GetSpools().ToArray();
                tempcount = data.Length;

                Assert.NotNull(data.FirstOrDefault(r => r.Id == tempid));
            }

            using (var work = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var rep = work.SpoolRepository;
                rep.Add();
            }

            using (var work = _unitOfWorkFactory.CreateUnitOfWork())
            {
                CelloSpoolBase[] data2 = work.SpoolRepository.GetSpools().ToArray();

                Assert.AreEqual(tempcount, data2.Length);
            }
        }

        [Test, Order(3)]
        public void RemoveTest()
        {
            int count;
            List<CelloSpoolBase> data;

            using (var work = _unitOfWorkFactory.CreateUnitOfWork())
            {
                data = work.SpoolRepository.GetSpools().ToList();
                count = data.Count;
            }

            using (var work = _unitOfWorkFactory.CreateUnitOfWork())
            {
                work.SpoolRepository.Remove(data[0]);
            }

            using (var work = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var data2 = work.SpoolRepository.GetSpools().ToArray();
                Assert.AreEqual(count, data2.Length);

                work.SpoolRepository.Remove(data[0]);

                work.Commit();
            }

            using (var work = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var data2 = work.SpoolRepository.GetSpools().ToArray();
                Assert.AreNotEqual(count, data2.Length);
            }
        }
        
        private static void Fill(CelloSpoolBase spool)
        {
            spool.Neededamount = _dataShuffler.GetNumber();
            spool.Amount = _dataShuffler.GetNumber(spool.Neededamount);
            spool.Name = _dataShuffler.GetName();
            spool.Type = _dataShuffler.GetType();
        }
    }
}