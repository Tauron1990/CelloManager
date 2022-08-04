using System.Collections.Immutable;
using System.Reactive;
using Game.Engine.Packageing.ScriptHosting.Scripts;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;

namespace Game.Engine.Packageing.ScriptHosting;

public sealed class PackageScriptManager
{
    private readonly GameScriptManager _gameScriptManager;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private readonly string _scriptDictionary;

    private ImmutableDictionary<string, Script> _cache = ImmutableDictionary<string, Script>.Empty;
    private ScriptState? _currentState;
    private ImmutableDictionary<string, string> _scripts = ImmutableDictionary<string, string>.Empty;

    public PackageScriptManager(string scriptDictionary, GameScriptManager gameScriptManager)
    {
        _scriptDictionary = scriptDictionary;
        _gameScriptManager = gameScriptManager;
    }

    public async ValueTask<PackageScriptManager> Init(GlobalScriptVariables scriptVariables)
    {
        var filePath = Path.Combine(_scriptDictionary, "scripts.json");
        if (!File.Exists(filePath)) return this;

        _currentState = await _gameScriptManager.CreateRootState(scriptVariables);

        var scriptDic = JsonConvert.DeserializeObject<ImmutableDictionary<string, string>>(await File.ReadAllTextAsync(filePath));

        _scripts =
        (
            from pair in scriptDic
            let fullPath = Path.Combine(_scriptDictionary, pair.Value)
            where File.Exists(fullPath)
            select KeyValuePair.Create(pair.Key, fullPath)
        ).ToImmutableDictionary();


        await ProcessInitScripts();
        
        return this;
    }

    private async ValueTask ProcessInitScripts()
    {
        const string initScriptName = "init.csx";
        var entrys = _scripts.Where(pair => pair.Value.EndsWith(initScriptName)).ToImmutableArray();
        _scripts = _scripts.RemoveRange(entrys.Select(p => p.Key));
        if(entrys.IsEmpty) return;
        
        var startscript = entrys.Select(p => p.Value).FirstOrDefault(e => Path.GetFileName(e) == initScriptName);

        if (!string.IsNullOrWhiteSpace(startscript))
            await ProcessScript(async s =>
            {
                var scriptInst = _gameScriptManager.CreateScript<Unit>(await File.ReadAllTextAsync(startscript), s);
                return await scriptInst.RunFromAsync(s);
            });


        foreach (var script in 
                 from entry in entrys
                 where Path.GetFileName(entry.Value) != initScriptName
                 orderby entry.Key
                 select entry.Value)
            await ProcessScript(async s =>
            {
                var scriptInst = _gameScriptManager.CreateScript<Unit>(await File.ReadAllTextAsync(script), s);
                return await scriptInst.RunFromAsync(s);
            });
    }
    
    private async ValueTask<TResult> ProcessScript<TResult>(Func<ScriptState, ValueTask<ScriptState<TResult>>> processFunc)
    {
        if (_currentState == null) throw new InvalidOperationException("The Script Manager is not Initialized");

        using var _ = await _lock.WaitDisposableAsync();

        var newState = await processFunc(_currentState);
        _currentState = newState;
        return newState.ReturnValue;
    }

    public async ValueTask<TResult> RunScript<TResult>(string name)
    {
        Script<TResult>? script = null;

        if (_cache.TryGetValue(name, out var cacheScript) && cacheScript is Script<TResult> actualScript)
            script = actualScript;


        return await ProcessScript(
            async s =>
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