namespace CelloManager.Core.DataOperators;

public interface IUpdateAware<in TElement>
{
    void Update(TElement element);
}