using System.Collections.Immutable;
using System.Reactive;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;

namespace Game.Engine.Packageing.ScriptHosting;

public sealed class PackageScriptManager
{
    private readonly SemaphoreSlim _lock = new(0, 1);
    
    private readonly string _scriptDictionary;
    private readonly GameScriptManager _gameScriptManager;

    private ImmutableDictionary<string, Script> _cache = ImmutableDictionary<string, Script>.Empty;
    private ImmutableDictionary<string, string> _scripts = ImmutableDictionary<string, string>.Empty;
    private ScriptState? _currentState;

    public PackageScriptManager(string scriptDictionary, GameScriptManager gameScriptManager)
    {
        _scriptDictionary = scriptDictionary;
        _gameScriptManager = gameScriptManager;
    }

    public async ValueTask<PackageScriptManager> Init()
    {
        var filePath = Path.Combine(_scriptDictionary, "scripts.json");
        if (File.Exists(filePath))
        {
            var scriptDic = JsonConvert.DeserializeObject<ImmutableDictionary<string, string>>(await File.ReadAllTextAsync(filePath));
            _scripts =
            (
                from pair in scriptDic
                let fullPath = Path.Combine(_scriptDictionary, pair.Value)
                where File.Exists(fullPath)
                select KeyValuePair.Create(pair.Key, fullPath)
            ).ToImmutableDictionary();
        }
        
        return this;
    }

    private async ValueTask<TResult> ProcessScript<TResult>(Func<ScriptState, ValueTask<ScriptState<TResult>>> processFunc)
    {
        using var _ = await _lock.WaitDisposableAsync();
        _currentState ??= await _gameScriptManager.CreateRootState();

        var newState = await processFunc(_currentState);
        _currentState = newState;
        return newState.ReturnValue;
    }

    public async ValueTask<TResult> RunScript<TResult>(string name)
    {
        Script<TResult>? script = null;
        if (_cache.TryGetValue(name, out var cacheScript) && cacheScript is Script<TResult> actualScript)
            script = actualScript;

        return await ProcessScript(async s =>
        {
            if (script is null)
            {
                script = _gameScriptManager.CreateScript<TResult>(await File.ReadAllTextAsync(_scripts[name]), s);
                _cache = _cache.SetItem(name, script);
            }

            return await script.RunFromAsync(s);
        });
    }
}