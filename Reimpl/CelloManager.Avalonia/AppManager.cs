using Material.Styles.Themes.Base;
using ReactiveUI;

namespace CelloManager;

public class AppManager : ReactiveObject
{
    public static AppManager Instance { get; } = new();
    
    private BaseThemeMode _themeMode;

    public BaseThemeMode ThemeMode
    {
        get => _themeMode;
        set => this.RaiseAndSetIfChanged(ref _themeMode, value);
    }

    public AppManager()
    {
        ThemeMode = BaseThemeMode.Dark;
    }
}