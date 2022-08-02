namespace Game.Engine.Screens;

public interface IScreenManager
{
    void Switch(string screen, object? parameter = null, Action? runSync = null);

    void Shutdown();
}