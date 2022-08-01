using System.Collections.Immutable;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Game.Engine.Packageing;

[PublicAPI]
public sealed class GameDataManager
{
    private readonly string _loadingRoot;

    public ImmutableList<InternalGamePackage> Packages { get; private set; } = ImmutableList<InternalGamePackage>.Empty;
    
    
    public GameDataManager(string loadingRoot)
    {
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
            Packages = Packages.Add(new InternalGamePackage(directory, pack));
        }
    }

    
    
    
    private async ValueTask<FilteredPackages> LoadFilter()
    {
        var file = Path.Combine(_loadingRoot, "EcludePackages.json");
        if (File.Exists(file))
            return JsonConvert.DeserializeObject<FilteredPackages>(await File.ReadAllTextAsync(file)) ?? new FilteredPackages(ImmutableList<string>.Empty);
        return new FilteredPackages(ImmutableList<string>.Empty);
    }
}