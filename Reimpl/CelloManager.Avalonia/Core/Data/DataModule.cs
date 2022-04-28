using Jab;

namespace CelloManager.Avalonia.Core.Data;

[ServiceProviderModule]
[Singleton(typeof(SpoolRepository))]
[Singleton(typeof(ErrorDispatcher))]
public interface IDataModule
{
    
}