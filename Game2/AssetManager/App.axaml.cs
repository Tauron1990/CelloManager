using AssetManager.Models;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AssetManager.ViewModels;
using AssetManager.Views;
using Material.Styles;
using Material.Styles.Themes.Base;
using Splat;

namespace AssetManager
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            RegisterSplat();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    ViewModel = Locator.Current.GetService<MainWindowViewModel>(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static void RegisterSplat()
        {
            Akavache.Registrations.Start("TauronAssetManager");
            
            SplatRegistrations.SetupIOC();
            SplatRegistrations.Register<ScriptsViewModel>();
            SplatRegistrations.Register<AssetsViewModel>();
            SplatRegistrations.RegisterLazySingleton<AppConfiguration>();
            SplatRegistrations.RegisterConstant(Akavache.BlobCache.UserAccount);
            SplatRegistrations.RegisterLazySingleton<MainWindowViewModel>();
        }
    }
}