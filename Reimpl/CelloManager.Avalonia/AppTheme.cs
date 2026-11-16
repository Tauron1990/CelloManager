using System;
using Material.Styles.Themes;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

#pragma warning disable IL2026

namespace CelloManager;

public sealed class AppTheme : MaterialThemeBase
{
    private readonly AppManager _appManager;
    
    public AppTheme(IServiceProvider? serviceProvider)
        : base(serviceProvider)
    {
        _appManager = Bootstrapper.ServiceProvider.GetRequiredService<AppManager>();
        _appManager
           .WhenAny(m => m.CurrentTheme, observedChange => observedChange.Value)
           .Subscribe(t => CurrentTheme = t);
    }

    protected override ITheme ProvideInitialTheme() => _appManager.CurrentTheme;
}