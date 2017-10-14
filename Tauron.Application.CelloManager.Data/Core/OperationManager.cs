using System;
using System.Collections.Generic;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Data.Core
{
    [Export(typeof(IOperationManager))]
    public class OperationManager : IOperationManager
    {
        private readonly object _lock = new object();

        private int _count;
        private bool _fail;
        private bool _commit;
        private List<Exception> _exception;
        private IOperation _currentOperation;
        private CoreDatabase _coreDatabase;

        public void Enter(Action<IOperation> operation)
        {
            lock (_lock)
            {
                _count++;

                try
                {
                    operation(GetOperation());
                }
                catch (Exception e)
                {
                    if(_exception == null)
                        _exception = new List<Exception>();

                    _exception.Add(e);
                    return;
                }

                if(_count == 0)
                    throw new InvalidOperationException($"Error in Operation Count State: {_count}");
                _count--;

                if (_count != 0) return;

                try
                {
                    if (_exception != null && _exception.Count != 0)
                        throw new AggregateException(_exception);

                    if(_fail) return;

                    if (_commit)
                        _coreDatabase.SaveChanges();

                }
                finally
                {
                    DisposeDatabase();
                }
            }
        }

        private void DisposeDatabase()
        {
            _currentOperation = null;
            _coreDatabase.Dispose();
            _coreDatabase = null;
            _exception = null;
        }

        private IOperation GetOperation()
        {
            if (_currentOperation != null) return _currentOperation;

            _coreDatabase = new CoreDatabase();
            _currentOperation = new Operation(new OptionsRepository(_coreDatabase), new SpoolRepository(_coreDatabase), new CommittedRefillRepository(_coreDatabase), 
                                                () => _commit = true, () => _fail = true);

            _commit = false;
            _fail = false;

            return _currentOperation;
        }

    }
}