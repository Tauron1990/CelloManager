using Jab;

namespace CelloManager.Core.Logic;

[ServiceProviderModule]
[Scoped(typeof(SpoolManager))]
[Scoped(typeof(OrderManager))]
public interface ILogicModule
{
    
}