using JetBrains.Annotations;

namespace Tauron.Application.CelloManager.Data.Core
{
    public interface IManagerEnviroment
    {
        [NotNull]
        ISettings Settings { get; }

        void Save();
    }
}