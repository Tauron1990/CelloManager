using System.Collections.Immutable;
using Game.Engine.Packageing.Files;
using Game.Engine.Packageing.ScriptHosting;
using Game.Engine.Packageing.ScriptHosting.Scripts;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Game.Engine.Packageing;

[PublicAPI]
public sealed class GameDataManager
{
    private readonly string _loadingRoot;

    public ImmutableList<InternalGamePackage> Packages { get; private set; } = ImmutableList<InternalGamePackage>.Empty;

    public GlobalScriptVariables Globals { get; } = new();
    
    public GameScriptManager ScriptManager { get; }

    public GameContentManager ContentManager { get; } = new();
    
    public GameDataManager(string loadingRoot)
    {
        ScriptManager = new GameScriptManager(Globals);
        _loadingRoot = loadingRoot;
    }

    public async ValueTask InitManager()
    {
        var toFilter = await LoadFilter();
        foreach (var directory in Directory.EnumerateDirectories(_loadingRoot))
        {
            var infoFilePath = Path.Combine(directory, "info.json");
            if(!File.Exists(infoFilePath))
                continue;
            var pack = JsonConvert.DeserializeObject<GamePackage>(await File.ReadAllTextAsync(infoFilePath));
            if(pack == null || toFilter.ExcludePackages.Contains(pack.Name))
                continue;
            Packages = Packages.Add(await CreateInternalPackage(directory, pack));
        }
    }

    private async ValueTask<InternalGamePackage> CreateInternalPackage(string sourcePath, GamePackage gamePackage)
        => new(
            sourcePath,
            gamePackage,
            await new PackageScriptManager(Path.Combine(sourcePath, "Scripts"), ScriptManager).Init(),
            ContentManager.Register(new PackageContentManager(Path.Combine(sourcePath, "Data")))
        );
    
    
    private async ValueTask<FilteredPackages> LoadFilter()
    {
        var file = Path.Combine(_loadingRoot, "EcludePackages.json");
        if (File.Exists(file))
            return JsonConvert.DeserializeObject<FilteredPackages>(await File.ReadAllTextAsync(file)) ?? new FilteredPackages(ImmutableList<string>.Empty);
        return new FilteredPackages(ImmutableList<string>.Empty);
    }
}