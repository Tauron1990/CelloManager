using JetBrains.Annotations;

namespace Tauron.Application.CelloManager.Data.Core
{
    public interface IManagerEnvioment
    {
        [NotNull]
        ISettings Settings { get; }

        void Save();
    }
}