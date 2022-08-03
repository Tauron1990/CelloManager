using System.Collections.Concurrent;
using Game.Engine.Packageing.ScriptHosting.Scripts;
using Terminal.Gui;

namespace Game.Engine.Screens;

public sealed class ScreenManager : Toplevel, IScreenManager
{
    private Action<IScreenManager>? _onReady;
    
    private readonly GameManager _gameManager;
    private readonly ConcurrentDictionary<string, IScreen> _screens = new();
    private IScreen? _currentScreen;

    public ScreenManager(Action<IScreenManager> onReady)
    {
        _onReady = onReady;
        GlobalScriptVariables.ScreenManager = this;
        
        LayoutStyle = LayoutStyle.Computed;
        Width = Dim.Fill();
        Height = Dim.Fill();

        _gameManager = new GameManager(this);
    }

    public override void OnLoaded()
    {
        base.OnLoaded();

        Task.Run(
            () => Application.MainLoop.Invoke(
                () =>
                {
                    _onReady?.Invoke(this);
                    _onReady = null;
                }));
    }

    public void RegisterScreen(string name, IScreen screen) => _screens[name] = screen;

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