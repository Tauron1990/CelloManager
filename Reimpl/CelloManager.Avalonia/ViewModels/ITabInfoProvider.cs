using ReactiveUI;

namespace CelloManager.ViewModels;

public interface ITabInfoProvider : IReactiveObject
{
    public string Title { get; }
    
    public bool CanClose { get; }
}