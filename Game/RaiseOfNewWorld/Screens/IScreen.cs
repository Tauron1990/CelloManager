using RaiseOfNewWorld.Engine;
using Terminal.Gui;

namespace RaiseOfNewWorld.Screens;

public interface IScreen
{
    void Setup(Window container, GameManager gameManager, object? parameter);

    void Teardown(GameManager gameManager);
}

public abstract class ScreenBase : IScreen
{
    public abstract void Setup(Window container, GameManager gameManager, object? parameter);

    public virtual void Teardown(GameManager gameManager)
    {
    }
}