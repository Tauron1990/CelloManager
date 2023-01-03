using Jab;

namespace CelloManager.Core.Data;

[ServiceProviderModule]
[Singleton<SpoolRepository>]
[Singleton<ErrorDispatcher>]
public interface IDataModule
{
    
}