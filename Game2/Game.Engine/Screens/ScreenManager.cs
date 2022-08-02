using Terminal.Gui;

namespace Game.Engine.Screens;

public sealed class ScreenManager : Toplevel, IScreenManager
{
    private readonly GameManager _gameManager;
    private IScreen? _currentScreen;

    public ScreenManager()
    {
        LayoutStyle = LayoutStyle.Computed;
        Width = Dim.Fill();
        Height = Dim.Fill();

        _gameManager = new GameManager(this);
    }

    public void Switch(string screen, object? parameter = null, Action? runSync = null)
    {
        Application.MainLoop.Invoke(
            () =>
            {
                RemoveAll();
                _currentScreen?.Teardown(_gameManager);

                var window = new Window();
                _currentScreen = _screens[screen];
                _currentScreen.Setup(
                    window,
                    _gameManager,
                    parameter);

                Add(window);

                runSync?.Invoke();
            });
    }

    public void Shutdown()
    {
        Application.MainLoop.Invoke(
            () =>
            {
                _gameManager.StopApplication();
                RequestStop();
            });
    }
}