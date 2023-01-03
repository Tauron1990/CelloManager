using Jab;

namespace CelloManager.Core.Comp;

[ServiceProviderModule]
[Transient(typeof(CoreDatabase))]
public interface ICompModule
{
    
}