using System.Collections.Immutable;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace Game.Engine.Packageing.Files;

[PublicAPI]
public sealed class PackageContentManager : IContentManager
{
    private ImmutableList<IContentProvider> _providers = ImmutableList<IContentProvider>.Empty;

    public PackageContentManager(string rootDirectory)
    {
        Registerprovider(new DataFileContentProvider(rootDirectory));
    }

    public bool Registerprovider(IContentProvider provider) => ImmutableInterlocked.Update(ref _providers, (list, prov) => list.Add(prov), provider);

    public async ValueTask Init() => await Task.WhenAll(_providers.Select(p => p.Init()));

    public bool CanOpen(string name) => _providers.Any(p => p.CanOpen(name));

    public Stream OpenData(string name) => _providers.Where(p => p.CanOpen(name)).Select(cp => cp.Open(name)).First();

    private JToken GetJson(string name)
    {
        using var reader = new StreamReader(OpenData(name));
        return JToken.Parse(reader.ReadToEnd());
    }

    private string TranslateJsonString(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;


        if (!input.StartsWith('$'))
            return input;


        var path = input[1..];
        using var reader = new StreamReader(_providers.First(c => c.CanOpenPath(path)).OpenPath(path));
        return reader.ReadToEnd();
    }
    
    public string GetString(string resourceName, string entryName) => TranslateJsonString(GetJson(resourceName).Value<string>(entryName));

    public int GetInt(string resourceName, string entryName) => GetJson(resourceName).Value<int>(entryName);
    public DateTime GetDateTime(string resourceName, string entryName) => GetJson(resourceName).Value<DateTime>(entryName);

    public string GetFileAsString(string resourceName)
    {
        using var reader = new StreamReader(OpenData(resourceName));
        return reader.ReadToEnd();
    }
}