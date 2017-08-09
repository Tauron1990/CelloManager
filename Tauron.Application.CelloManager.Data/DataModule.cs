using Tauron.Application.CelloManager.Data.Core;

namespace Tauron.Application.CelloManager.Data
{
    [ExportModule]
    public class DataModule : IModule
    {
        public int Order { get; } = int.MinValue;

        public void Initialize(CommonApplication application)
        {
            using (var db = new CoreDatabase())
                db.UpdateSchema();
        }
    }
}