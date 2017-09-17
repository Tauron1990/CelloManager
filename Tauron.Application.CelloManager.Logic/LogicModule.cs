using System;
using Tauron.Application.CelloManager.Logic.Historie;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Logic
{
    [ExportModule]
    public class LogicModule : IModule
    {
        [Inject]
        public Lazy<ICommittedRefillManager> CommittedRefillManager { get; set; }

        public int Order { get; } = -1;

        public void Initialize(CommonApplication application)
        {
            CommittedRefillManager.Value.Purge();
        }
    }
}