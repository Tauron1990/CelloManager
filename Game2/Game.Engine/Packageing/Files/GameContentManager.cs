using System.Collections.Immutable;

namespace Game.Engine.Packageing.Files;

public interface IContentManager
{
    bool CanOpen(string resourceName);

    Stream OpenData(string resourceName);
    
    string GetString(string resourceName, string entryName);
    int GetInt(string resourceName, string entryName);
    DateTime GetDateTime(string resourceName, string entryName);

    string GetFileAsString(string resourceName);
}

public sealed class GameContentManager : IContentManager
{
    private ImmutableList<PackageContentManager> _contentManagers = ImmutableList<PackageContentManager>.Empty;

    public async ValueTask<PackageContentManager> Register(PackageContentManager gameContentManager)
    {
        await gameContentManager.Init();
        ImmutableInterlocked.Update(ref _contentManagers, (l, m) => l.Add(m), gameContentManager);

        return gameContentManager;
    }

    public bool CanOpen(string resourceName) => _contentManagers.Any(cm => cm.CanOpen(resourceName));

    private PackageContentManager GetManager(string name) => _contentManagers.First(cm => cm.CanOpen(name));
    
    public Stream OpenData(string resourceName) => GetManager(resourceName).OpenData(resourceName);

    public string GetString(string resourceName, string entryName) => GetManager(resourceName).GetString(resourceName, entryName);

    public int GetInt(string resourceName, string entryName) => GetManager(resourceName).GetInt(resourceName, entryName);
    public DateTime GetDateTime(string resourceName, string entryName) => GetManager(resourceName).GetDateTime(resourceName, entryName);
    public string GetFileAsString(string resourceName) => GetManager(resourceName).GetFileAsString(resourceName);
}