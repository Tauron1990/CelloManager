using Game.Engine.Screens;
using RaiseOfNewWorld;
using RaiseOfNewWorld.Screens;
using Terminal.Gui;

Console.Title = "Rise of New World";

Application.Run(new ScreenManager(InitApp));
Application.Shutdown();

void InitApp(IScreenManager manager)
{
    manager.RegisterScreen(nameof(MainScreen), new MainScreen());
    manager.RegisterScreen(nameof(LoadingScreen), new LoadingScreen());
    manager.RegisterScreen(nameof(LoadGameScreen), new LoadGameScreen());
    
    manager.Switch(nameof(LoadingScreen), new LoadingParameter(GameBootstrap.InitialieGame, -1, "Spiel Starten"));
}