using System;
using System.Diagnostics.CodeAnalysis;

namespace Tauron.Application.CelloManager.Core
{
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public class ActionDispose : IDisposable
    {
        private readonly Action _action;

        public ActionDispose(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }
}