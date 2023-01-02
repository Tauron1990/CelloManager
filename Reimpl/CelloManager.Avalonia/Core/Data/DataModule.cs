using Jab;

namespace CelloManager.Avalonia.Core.Data;

[ServiceProviderModule]
[Singleton<SpoolRepository>]
[Singleton<ErrorDispatcher>]
public interface IDataModule
{
    
}