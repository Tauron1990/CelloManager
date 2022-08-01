using System.Collections.Immutable;
using System.Reactive;
using System.Reflection;
using System.Runtime.InteropServices;
using Game.Engine.Packageing.ScriptHosting.Scripts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Game.Engine.Packageing.ScriptHosting;

public sealed class GameScriptManager
{
    private readonly GlobalScriptVariables _scriptVariables;
    private readonly Script<Unit> _rootScript;

    public GameScriptManager(GlobalScriptVariables scriptVariables)
    {
        var startPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        var scriptConfig = ScriptOptions.Default
            .WithMetadataResolver(ScriptMetadataResolver.Default.WithSearchPaths(RuntimeEnvironment.GetRuntimeDirectory(), startPath))
            .WithSourceResolver(new SourceFileResolver(ImmutableArray<string>.Empty, Path.Combine(startPath, "scripts")))
            .WithReferences(typeof(IEntryPoint).Assembly, typeof(Observer).Assembly)
            .AddReferences(Assembly.GetExecutingAssembly())
            .WithOptimizationLevel(OptimizationLevel.Release)
            .WithImports(
                "System",
                "System.Collections.Generic",
                "System.Linq",
                "System.Threading.Tasks",
                "System.Reactive",
                "Game.Engine");


#if DEBUG
        scriptConfig = scriptConfig.WithOptimizationLevel(OptimizationLevel.Debug);
#endif

        _rootScript = CSharpScript.Create<Unit>("Unit.Default", scriptConfig, typeof(GlobalScriptVariables));
        _scriptVariables = scriptVariables;
    }

    public Task<ScriptState<Unit>> CreateRootState()
        => _rootScript.RunAsync(_scriptVariables);

    public Script<TResult> CreateScript<TResult>(string code, ScriptState state)
        => CSharpScript.Create<TResult>(code, state.Script.Options, typeof(GlobalScriptVariables));
}