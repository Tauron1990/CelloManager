using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Tauron.Application.CelloManager.Data.Core;
using TestHelpers;

// ReSharper disable once CheckNamespace
namespace Tauron.Application.CelloManager.Data.Historie.Tests
{
    [TestFixture, NonParallelizable]
    public class CommittedRefillRepositoryTests
    {
        private static readonly object Block = new object();
        private readonly DataShuffler _dataShuffler = new DataShuffler();
        private IOperationManager _operationManager;

        [OneTimeSetUp]
        public void Initialize()
        {
            lock (Block)
            {
                InitializeHelper.Initialize();
                
                CoreDatabase.OverrideConnection(DBHelper.DbConstring);
                using (CoreDatabase db = new CoreDatabase())
                    db.UpdateSchema();

                _operationManager = new OperationManager();
            }
        }

        [Test, Order(1)]
        public void GetCommittedRefillsTest()
        {
            lock (Block)
            {
                _operationManager.Enter(o =>
                {
                    CommittedRefill[] data = o.CommittedRefills.GetCommittedRefills().ToArray();
                    Assert.AreEqual(4, data.Length);
                });
            }
        }

        [Test, Order(2)]
        public void AddTest()
        {
            lock (Block)
            {
                int tempId = 0;
                _operationManager.Enter(p =>
                {
                    CommittedRefill refillToAdd = CreateCommittedRefill();

                    p.CommittedRefills.Add(refillToAdd);
                    p.Commit();
                    tempId = refillToAdd.Id;
                });

                int tempLenght = 0;
                _operationManager.Enter(p =>
                {
                    CommittedRefill[] data = p.CommittedRefills.GetCommittedRefills().ToArray();
                    tempLenght = data.Length;
                    Assert.NotNull(data.FirstOrDefault(r => r.Id == tempId));
                });

                _operationManager.Enter(p =>
                {
                    p.CommittedRefills.Add(CreateCommittedRefill());
                });

                _operationManager.Enter(p =>
                {
                    CommittedRefill[] data2 = p.CommittedRefills.GetCommittedRefills().ToArray();

                    Assert.AreEqual(tempLenght, data2.Length);
                });
            }
        }

        [Test, Order(3)]
        public void DeleteTest()
        {
            lock (Block)
            {
                int count = 0;
                List<CommittedRefill> data = new List<CommittedRefill>();

                _operationManager.Enter(p =>
                {
                    data = p.CommittedRefills.GetCommittedRefills().ToList();
                    count = data.Count;
                });

                _operationManager.Enter(p =>
                {
                    p.CommittedRefills.Delete(data[0]);
                });

                _operationManager.Enter(p =>
                {
                    CommittedRefill[] data2 = p.CommittedRefills.GetCommittedRefills().ToArray();
                    Assert.AreEqual(count, data2.Length);

                    p.CommittedRefills.Delete(data2[0]);

                    p.Commit();
                });

                _operationManager.Enter(p =>
                {
                    var data2 = p.CommittedRefills.GetCommittedRefills().ToArray();
                    Assert.AreNotEqual(count, data2.Length);
                });
            }
        }

        private CommittedRefill CreateCommittedRefill()
        {
            return new CommittedRefill {SentTime = DateTime.Now, CommitedSpools = {new CommittedSpool(_dataShuffler.GetName(), _dataShuffler.GetNumber(), _dataShuffler.GetType(), _dataShuffler.GetNumber())}};
        }
    }
}