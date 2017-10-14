using System;

namespace Tauron.Application.CelloManager.Data
{
    public interface IOperationManager
    {
        void Enter(Action<IOperation> operation);
    }
}