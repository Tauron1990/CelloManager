using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

namespace RaiseOfNewWorld.Engine.Data;

public abstract class ContentManager
{
    private sealed class EmptyManager : ContentManager
    {
        public override DateTime GetDateTime(string name, string? fileName = null) => throw new NotImplementedException();

        public override int GetInt(string name, string? fileName = null) => throw new NotImplementedException();

        public override string GetString(string name, string? fileName = null) => throw new NotImplementedException();

        public override Func<string> GetStringFunc(string name, string? fileName = null) => throw new NotImplementedException();
        public override string ReadFile(string relativeFileName) => throw new NotImplementedException();
    }

    public static readonly ContentManager Empty = new EmptyManager();

    public abstract DateTime GetDateTime(string name, [CallerFilePath] string? fileName = null);

    public abstract int GetInt(string name, [CallerFilePath] string? fileName = null);

    public abstract string GetString(string name, [CallerFilePath] string? fileName = null);

    public abstract Func<string> GetStringFunc(string name, [CallerFilePath] string? fileName = null);

    public abstract string ReadFile(string relativeFileName);

    public string GetString(TextAttributeValue attributeValue) 
        => attributeValue.IsReference ? ReadFile(attributeValue.Value) : attributeValue.Value;
}

public sealed class FileContentManager : ContentManager
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

    public override DateTime GetDateTime(string name, [CallerFilePath] string? fileName = null) => GetToken(name, fileName).Value<DateTime>();

    public override  int GetInt(string name, [CallerFilePath] string? fileName = null) => GetToken(name, fileName).Value<int>();

    public override string GetString(string name, [CallerFilePath] string? fileName = null)
    {
        var value = GetToken(name, fileName).Value<string>() ?? string.Empty;
        return value.StartsWith("$") ? File.ReadAllText(Path.Combine(BaseDirectory, value[1..])) : value;
    }

    public override Func<string> GetStringFunc(string name, [CallerFilePath] string? fileName = null)
        => () =>
        {
            var value = GetToken(name, fileName).Value<string>() ?? string.Empty;
            return value.StartsWith("$") ? File.ReadAllText(Path.Combine(BaseDirectory, value[1..])) : value;
        };

    public override string ReadFile(string relativeFileName)
        => File.ReadAllText(Path.Combine(BaseDirectory, relativeFileName));
}