using System;
using Material.Styles.Themes;
using ReactiveUI;

namespace CelloManager;

public sealed class AppTheme : MaterialThemeBase
{
    public AppTheme(IServiceProvider? serviceProvider) 
        : base(serviceProvider)
    {
        AppManager.Instance
                  .WhenAny(m => m.CurrentTheme, observedChange => observedChange.Value)
                  .Subscribe(t => CurrentTheme = t);
    }

    protected override ITheme? ProvideInitialTheme() => AppManager.Instance.CurrentTheme;
}