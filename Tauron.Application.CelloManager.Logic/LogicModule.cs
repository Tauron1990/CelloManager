using System;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Logic.Historie;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Logic
{
    [ExportModule]
    public class LogicModule : IModule
    {
        [Inject]
        public Lazy<IManagerEnviroment> ManagerEnviroment { get; set; }

        [Inject]
        public Lazy<ICommittedRefillManager> CommittedRefillManager { private get; set; }

        public int Order { get; } = -1;

        public void Initialize(CommonApplication application)
        {
            if(ManagerEnviroment.Value.Settings.Purge)
                CommittedRefillManager.Value.Purge();
        }
    }
}