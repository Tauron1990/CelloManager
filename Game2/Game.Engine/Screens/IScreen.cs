using Terminal.Gui;

namespace Game.Engine.Screens;

public interface IScreen
{
    void Setup(Window container, GameManager gameManager, object? parameter);

    void Teardown(GameManager gameManager);
}