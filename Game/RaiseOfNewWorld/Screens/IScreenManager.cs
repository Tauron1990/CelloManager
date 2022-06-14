namespace RaiseOfNewWorld.Screens;

public interface IScreenManager
{
    void Switch(string screen, object? parameter = null);

    void Shutdown();
}