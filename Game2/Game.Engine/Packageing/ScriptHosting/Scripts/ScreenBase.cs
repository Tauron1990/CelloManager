using Game.Engine.Screens;
using Terminal.Gui;

namespace Game.Engine.Packageing.ScriptHosting.Scripts;

public abstract class ScreenBase : IScreen
{
    public abstract void Setup(Window container, GameManager gameManager, object? parameter);

    public virtual void Teardown(GameManager gameManager) { }
}