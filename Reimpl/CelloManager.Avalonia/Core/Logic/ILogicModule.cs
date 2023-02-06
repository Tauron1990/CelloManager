using Jab;

namespace CelloManager.Core.Logic;

[ServiceProviderModule]
[Scoped<SpoolManager>]
[Scoped<OrderManager>]
[Scoped<SpoolPriceManager>]
public interface ILogicModule
{
    
}