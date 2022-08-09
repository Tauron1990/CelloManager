using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AssetManager.Models;
using JetBrains.Annotations;
using ReactiveUI;
using Splat;

namespace AssetManager.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly AppConfiguration _appConfiguration;
        private string _currentPath = string.Empty;

        public ScriptsViewModel Scripts { get; }

        public AssetsViewModel Assets { get; }

        public string CurrentPath
        {
            get => _currentPath;
            set => this.RaiseAndSetIfChanged(ref _currentPath, value);
        }

        public MainWindowViewModel()
        {
            Scripts = null!;
            Assets = null!;
            _appConfiguration = null!;
        }
        
        [DependencyInjectionConstructor, UsedImplicitly]
        public MainWindowViewModel(AppConfiguration appConfiguration, ScriptsViewModel scriptsViewModel, AssetsViewModel assetsViewModel)
        {
            _appConfiguration = appConfiguration;
            Scripts = scriptsViewModel;
            Assets = assetsViewModel;

            this.WhenActivated(InitModel);
        }

        private IEnumerable<IDisposable> InitModel()
        {
            yield return this.WhenAny(m => m.CurrentPath, m => m.Value)
                .Throttle(TimeSpan.FromSeconds(5))
                .SelectMany(
                    s => Observable.Return(s)
                        .SelectMany(LoadNewData)
                        .CatchAndDisplayError())
                .Subscribe();


            yield return _appConfiguration.WhenAny(c => c.CurrentLoaded, c => c.Value)
                .StartWith(_appConfiguration.CurrentLoaded)
                .DistinctUntilChanged()
                .ObservOnDispatcher()
                .Take(1)
                .Subscribe(s => CurrentPath = s);
        }

        private async Task<Unit> LoadNewData(string path)
        {
            Scripts.Reset();
            Assets.Reset();
            
            if(!Directory.Exists(path)) return Unit.Default;

            _appConfiguration.CurrentLoaded = path;
            
            try
            {
                await Scripts.Load(path);
                await Assets.Load(path);
            }
            catch (Exception)
            {
                Scripts.Reset();
                Assets.Reset();
                throw;
            }
            return Unit.Default;
        }
    }
}