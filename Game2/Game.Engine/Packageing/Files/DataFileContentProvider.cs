using System.Collections.Immutable;
using Newtonsoft.Json;

namespace Game.Engine.Packageing.Files;

public sealed class DataFileContentProvider : IContentProvider
{
    private readonly string _rootDirectory;
    private ImmutableDictionary<string, string> _entrys = ImmutableDictionary<string, string>.Empty;

    public DataFileContentProvider(string rootDirectory)
    {
        _rootDirectory = rootDirectory;
    }

    public async Task Init()
    {
        var filePath = Path.Combine(_rootDirectory, "data.json");

        var scriptDic = JsonConvert.DeserializeObject<ImmutableDictionary<string, string>>(await File.ReadAllTextAsync(filePath));

        _entrys =
        (
            from pair in scriptDic
            let fullPath = Path.Combine(_rootDirectory, pair.Value)
            where File.Exists(fullPath)
            select KeyValuePair.Create(pair.Key, fullPath)
        ).ToImmutableDictionary();
    }

    public bool CanOpen(string name) => _entrys.ContainsKey(name);

    public Stream Open(string name) => File.OpenRead(_entrys[name]);
}