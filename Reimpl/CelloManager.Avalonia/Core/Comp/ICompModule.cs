using Jab;

namespace CelloManager.Avalonia.Core.Comp;

[ServiceProviderModule]
[Transient(typeof(CoreDatabase))]
public interface ICompModule
{
    
}