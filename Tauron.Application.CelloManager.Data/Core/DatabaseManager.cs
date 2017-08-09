using System;
using Tauron.Application.CelloManager.Core;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Data.Core
{
    [Export(typeof(IDatabaseManager)), Shared]
    public sealed class DatabaseManager : IDatabaseManager
    {
        private int _operationCount;

        public CoreDatabase Database { get; private set; }

        public bool SaveChanges { get; set; }

        public IDisposable StartOperation()
        {
            _operationCount++;

            if (_operationCount == 1)
            {
                SaveChanges = true;
                Database = new CoreDatabase();
            }

            return new ActionDispose(() =>
            {
                _operationCount--;

                if (_operationCount != 0) return;

                if (SaveChanges)
                    Database.SaveChanges();
                Database.Dispose();
            });
        }
    }
}