using System.Collections.Immutable;
using Game.Engine.Core;
using Game.Engine.Packageing.Files;
using Game.Engine.Packageing.ScriptHosting;
using Game.Engine.Packageing.ScriptHosting.Scripts;
using Game.Engine.Screens;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Terminal.Gui;

namespace Game.Engine.Packageing;

[PublicAPI]
public sealed class GameDataManager
{
    private readonly string _loadingRoot;
    private readonly GameManager _gameManager;

    public GameDataManager(string loadingRoot, GameManager gameManager)
    {
        ScriptManager = new GameScriptManager();
        _loadingRoot = loadingRoot;
        _gameManager = gameManager;
    }

    public ImmutableList<InternalGamePackage> Packages { get; private set; } = ImmutableList<InternalGamePackage>.Empty;

    public ImmutableList<Func<UiExtensions.MenuItemFactory, IEnumerable<UiExtensions.MenuBarItemBuilder>>> MainMenuRegistrations { get; private set; }
        = ImmutableList<Func<UiExtensions.MenuItemFactory, IEnumerable<UiExtensions.MenuBarItemBuilder>>>.Empty;
    
    public GameScriptManager ScriptManager { get; }

    public GameContentManager ContentManager { get; } = new();

    public void RegisterMainMenuBuilder(Func<UiExtensions.MenuItemFactory, IEnumerable<UiExtensions.MenuBarItemBuilder>> builder)
        => MainMenuRegistrations = MainMenuRegistrations.Add(builder);
    
    public async ValueTask InitManager()
    {
        var toFilter = await LoadFilter();
        var paks = new Dictionary<string, (string Path, GamePackage Pack)>();
        
        foreach (var directory in Directory.EnumerateDirectories(_loadingRoot))
        {
            var infoFilePath = Path.Combine(directory, "info.json");

            if (!File.Exists(infoFilePath))
                continue;


            var pack = JsonConvert.DeserializeObject<GamePackage>(await File.ReadAllTextAsync(infoFilePath));

            if (pack == null || toFilter.ExcludePackages.Contains(pack.Name))
                continue;
            
            paks.Add(pack.Name, (directory, pack));
        }


        var cycles = paks.Select(gp => gp.Value.Pack).Select(gp => KeyValuePair.Create(gp.Name, gp.LoadBefore)).ToImmutableDictionary().FindCycles();

        if (cycles.Count != 0)
        {
            MessageBox.Query("Zyklische Referenz", string.Join("->", cycles[0]), "Beenden");
            Application.Shutdown();
            
            return;
        }


        await LoadPackages(paks);
        
        _gameManager.ScreenManager.Switch("GameScreen");
    }

    private async ValueTask LoadPackages(Dictionary<string, (string Path, GamePackage Pack)> paks)
    {
        var loaded = new HashSet<string>();

        foreach (var gamePackage in paks.Values)
        {
            foreach (var toLoad in gamePackage.Pack.LoadBefore.Where(toLoad => loaded.Add(toLoad)))
            {
                var toLoadPack = paks[toLoad];
                Packages = Packages.Add(await CreateInternalPackage(toLoadPack.Path, toLoadPack.Pack));
            }
            
            
            if (loaded.Add(gamePackage.Pack.Name))
                Packages = Packages.Add(await CreateInternalPackage(gamePackage.Path, gamePackage.Pack));
        }
    }
    
    private async ValueTask<InternalGamePackage> CreateInternalPackage(string sourcePath, GamePackage gamePackage)
    {
        var scriptManager = new PackageScriptManager(Path.Combine(sourcePath, "Scripts"), ScriptManager);
        var contentManager = await ContentManager.Register(new PackageContentManager(Path.Combine(sourcePath, "Data")));
        scriptManager = await scriptManager.Init(CreateVariables(scriptManager, contentManager));

        return new InternalGamePackage(
            sourcePath,
            gamePackage,
            scriptManager,
            contentManager
        );
    }

    private GlobalScriptVariables CreateVariables(PackageScriptManager scriptManager, PackageContentManager contentManager) =>
        new(scriptManager, contentManager, _gameManager);

    private async ValueTask<FilteredPackages> LoadFilter()
    {
        var file = Path.Combine(_loadingRoot, "EcludePackages.json");

        if (File.Exists(file))
            return JsonConvert.DeserializeObject<FilteredPackages>(await File.ReadAllTextAsync(file)) ??
                   new FilteredPackages(ImmutableList<string>.Empty);


        return new FilteredPackages(ImmutableList<string>.Empty);
    }
}