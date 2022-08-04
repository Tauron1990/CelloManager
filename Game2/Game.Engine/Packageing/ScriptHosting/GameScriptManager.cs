using System.Collections.Immutable;
using System.Reactive;
using System.Reflection;
using System.Runtime.InteropServices;
using EcsRx.Attributes;
using EcsRx.Infrastructure;
using EcsRx.Plugins.Bootstrap;
using EcsRx.Plugins.Computeds;
using EcsRx.Plugins.GroupBinding;
using EcsRx.Plugins.ReactiveSystems;
using EcsRx.ReactiveData;
using Game.Engine.Packageing.ScriptHosting.Scripts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using SystemsRx.Plugins.Computeds;

namespace Game.Engine.Packageing.ScriptHosting;

public sealed class GameScriptManager
{
    private readonly Script<Unit> _rootScript;

    public GameScriptManager()
    {
        var startPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        var scriptConfig = ScriptOptions.Default
            .WithMetadataResolver(ScriptMetadataResolver.Default.WithSearchPaths(RuntimeEnvironment.GetRuntimeDirectory(), startPath))
            .WithSourceResolver(new SourceFileResolver(ImmutableArray<string>.Empty, Path.Combine(startPath, "scripts")))
            .WithReferences
            (
                typeof(GlobalScriptVariables).Assembly,
                typeof(Observer).Assembly,
                typeof(SystemBootstrapPlugin).Assembly,
                typeof(ComputedFromGroup<>).Assembly,
                typeof(GroupBindingsPlugin).Assembly,
                typeof(ReactiveSystemsPlugin).Assembly,
                typeof(ReactiveProperty<>).Assembly,
                typeof(CollectionAffinityAttribute).Assembly,
                typeof(EcsRxApplication).Assembly
                )
            .AddReferences(Assembly.GetExecutingAssembly())
            .WithOptimizationLevel(OptimizationLevel.Release)
            .WithImports(
                "System",
                "System.Collections.Generic",
                "System.Linq",
                "System.Threading.Tasks",
                "System.Reactive",
                "System.Reactive.Linq",
                "EcsRx.Extension",
                "Game.Engine",
                "Game.Engine.Core",
                "Game.Engine.Core.Rooms",
                "Game.Engine.Core.Rooms.Types",
                "Game.Engine.Core.Movement",
                "Game.Engine.Core.Time",
                "Game.Engine.Screens",
                "Game.Engine.Packageing.ScriptHosting.Scripts",
                "Terminal.Gui"
                );
        
#if DEBUG
        scriptConfig = scriptConfig.WithOptimizationLevel(OptimizationLevel.Debug);
#endif

        _rootScript = CSharpScript.Create<Unit>("Unit.Default", scriptConfig, typeof(GlobalScriptVariables));
    }

    public Task<ScriptState<Unit>> CreateRootState(GlobalScriptVariables scriptVariables) => _rootScript.RunAsync(scriptVariables);

    public Script<TResult> CreateScript<TResult>(string code, ScriptState state) =>
        state.Script.ContinueWith<TResult>(code);
}