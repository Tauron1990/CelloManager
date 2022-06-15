using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;

namespace RaiseOfNewWorld.Engine.Data;

public sealed class ContentManager
{
    private static readonly string BaseDirectory = Path.GetFullPath("GameData");

    private readonly object _lock = new();
    private readonly Dictionary<string, JToken> _loadedData = new();

    private JToken GetToken(string name, string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new InvalidOperationException("No File Name Provided");

        JToken? propertyToken;

        lock (_lock)
        {
            if (_loadedData.TryGetValue(fileName, out var token))
                propertyToken = token[name];
            else
            {
                var targetFile = Path.Combine(BaseDirectory, Path.GetFileNameWithoutExtension(fileName)) + ".json";
                if (!File.Exists(targetFile))
                    throw new InvalidOperationException($"No File for Key Found: {targetFile}");

                token = JToken.Parse(File.ReadAllText(targetFile));
                _loadedData[fileName] = token;

                propertyToken = token[name];
            }
        }

        return propertyToken ?? throw new InvalidOperationException($"No Entry with name {name} Found");
    }

    public DateTime GetDateTime(string name, [CallerFilePath] string? fileName = null) => GetToken(name, fileName).Value<DateTime>();

    public int GetInt(string name, [CallerFilePath] string? fileName = null) => GetToken(name, fileName).Value<int>();

    public string GetString(string name, [CallerFilePath] string? fileName = null)
    {
        var value = GetToken(name, fileName).Value<string>() ?? string.Empty;
        return value.StartsWith("$") ? File.ReadAllText(Path.Combine(BaseDirectory, value[1..])) : value;
    }
}