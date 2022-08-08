using System.Collections.Immutable;
using System.Globalization;
using Newtonsoft.Json;

namespace Game.Engine.Packageing.Files;

public sealed class DataFileContentProvider : IContentProvider
{
    private readonly record struct DataEntry(string Simple, ImmutableDictionary<string, string> Entrys);
    
    private readonly string _rootDirectory;
    private ImmutableDictionary<string, DataEntry> _entrys = ImmutableDictionary<string, DataEntry>.Empty;

    public DataFileContentProvider(string rootDirectory) => _rootDirectory = rootDirectory;

    public async Task Init()
    {
        var filePath = Path.Combine(_rootDirectory, "data.json");
        var langdics = Directory.GetDirectories(_rootDirectory);
        
        var dataDic = JsonConvert.DeserializeObject<ImmutableDictionary<string, string>>(await File.ReadAllTextAsync(filePath));

        _entrys =
        (
            from pair in dataDic
            let fullPath = Path.Combine(_rootDirectory, pair.Value)
            where File.Exists(fullPath)
            select KeyValuePair.Create(
                pair.Key,
                new DataEntry(
                    fullPath,
                    (
                        from lang in langdics
                        let langFullPath = Path.Combine(_rootDirectory, lang, pair.Value)
                        where File.Exists(langFullPath)
                        select KeyValuePair.Create(lang, langFullPath)
                    ).ToImmutableDictionary()))
        ).ToImmutableDictionary();
    }

    
    
    public bool CanOpen(string name) => _entrys.ContainsKey(name);

    public Stream Open(string name)
    {
        var data = _entrys[name];

        return File.OpenRead(data.Entrys.TryGetValue(CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName, out var file) ? file : data.Simple);
    }

    public Stream OpenPath(string path) => File.OpenRead(Path.Combine(_rootDirectory, path));
    public bool CanOpenPath(string path) => File.Exists(Path.Combine(_rootDirectory, path));
}