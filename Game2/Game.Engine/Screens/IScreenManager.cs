namespace Game.Engine.Screens;

public interface IScreenManager
{
    void RegisterScreen(string name, IScreen screen);
    
    void Switch(string screen, object? parameter = null, Action? runSync = null);

    void Shutdown();
}