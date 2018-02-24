using System;
using System.Collections.Generic;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.CelloManager.Data
{
    [ExportRepositoryExtender]
    public sealed class Repositorys : CommonRepositoryExtender<CoreDatabase>
    {
        public override IEnumerable<(Type, Type)> GetRepositoryTypes()
        {
            yield return (typeof(ICommittedRefillRepository), typeof(CommittedRefillRepository));
            yield return (typeof(ISpoolRepository), typeof(SpoolRepository));
        }
    }
}