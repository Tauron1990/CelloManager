using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Data.Core.Tests;
using TestHelpers;

// ReSharper disable once CheckNamespace
namespace Tauron.Application.CelloManager.Data.Historie.Tests
{
    [TestFixture, NonParallelizable]
    public class CommittedRefillRepositoryTests
    {
        private static readonly object _block = new object();
        private DataShuffler _dataShuffler;
        private IUnitOfWorkFactory _unitOfWorkFactory;

        [OneTimeSetUp]
        public void Initialize()
        {
            lock (_block)
            {
                InitializeHelper.Initialize();

                _dataShuffler = new DataShuffler();

                CoreDatabase.OverrideConnection(DBHelper.DbConstring);
                using (var db = new CoreDatabase())
                    db.UpdateSchema();

                _unitOfWorkFactory = UnitOfWorkTests.CreateFactory(typeof(CommittedRefillRepository));

                using (var work = _unitOfWorkFactory.CreateUnitOfWork())
                {
                    for (int i = 0; i < 4; i++)
                    {
                        work.CommittedRefillRepository.Add(CreateCommittedRefill());
                    }

                    work.Commit();
                }
            }
        }

        [Test, Order(1)]
        public void GetCommittedRefillsTest()
        {
            lock (_block)
            {
                using (var work = _unitOfWorkFactory.CreateUnitOfWork())
                {
                    CommittedRefill[] data = work.CommittedRefillRepository.GetCommittedRefills().ToArray();
                    Assert.AreEqual(4, data.Length);
                }
            }
        }

        [Test, Order(2)]
        public void AddTest()
        {
            lock (_block)
            {
                int tempId;
                using (var work = _unitOfWorkFactory.CreateUnitOfWork())
                {
                    CommittedRefill refillToAdd = CreateCommittedRefill();

                    work.CommittedRefillRepository.Add(refillToAdd);
                    work.Commit();
                    tempId = refillToAdd.Id;
                }

                int tempLenght;
                using (var work = _unitOfWorkFactory.CreateUnitOfWork())
                {
                    CommittedRefill[] data = work.CommittedRefillRepository.GetCommittedRefills().ToArray();
                    tempLenght = data.Length;
                    Assert.NotNull(data.FirstOrDefault(r => r.Id == tempId));
                }

                using (var work = _unitOfWorkFactory.CreateUnitOfWork())
                {
                    work.CommittedRefillRepository.Add(CreateCommittedRefill());
                }

                using (var work = _unitOfWorkFactory.CreateUnitOfWork())
                {
                    CommittedRefill[] data2 = work.CommittedRefillRepository.GetCommittedRefills().ToArray();

                    Assert.AreEqual(tempLenght, data2.Length);
                }
            }
        }

        [Test, Order(3)]
        public void DeleteTest()
        {
            lock (_block)
            {
                int count;
                List<CommittedRefill> data;

                using (var work = _unitOfWorkFactory.CreateUnitOfWork())
                {
                    data = work.CommittedRefillRepository.GetCommittedRefills().ToList();
                    count = data.Count;
                }

                using (var work = _unitOfWorkFactory.CreateUnitOfWork())
                {
                    work.CommittedRefillRepository.Delete(data[0]);
                }

                using (var work = _unitOfWorkFactory.CreateUnitOfWork())
                {
                    CommittedRefill[] data2 = work.CommittedRefillRepository.GetCommittedRefills().ToArray();
                    Assert.AreEqual(count, data2.Length);

                    work.CommittedRefillRepository.Delete(data2[0]);

                    work.Commit();
                }

                using (var work = _unitOfWorkFactory.CreateUnitOfWork())
                {
                    var data2 = work.CommittedRefillRepository.GetCommittedRefills().ToArray();
                    Assert.AreNotEqual(count, data2.Length);
                }
            }
        }

        private CommittedRefill CreateCommittedRefill()
        {
            return new CommittedRefill {SentTime = DateTime.Now, CommitedSpools = {new CommittedSpool(_dataShuffler.GetName(), _dataShuffler.GetNumber(), _dataShuffler.GetType(), _dataShuffler.GetNumber())}};
        }
    }
}