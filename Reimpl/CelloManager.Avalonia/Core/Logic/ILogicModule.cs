using Jab;

namespace CelloManager.Avalonia.Core.Logic;

[ServiceProviderModule]
[Scoped(typeof(SpoolManager))]
[Scoped(typeof(OrderManager))]
public interface ILogicModule
{
    
}