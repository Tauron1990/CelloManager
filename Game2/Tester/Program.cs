// See https://aka.ms/new-console-template for more information


using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;
using Game.Engine.Packageing.ScriptHosting.Scripts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;

await Task.Delay(1000);

var startPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

var testConfig = ScriptOptions.Default
        .WithMetadataResolver(ScriptMetadataResolver.Default.WithSearchPaths(RuntimeEnvironment.GetRuntimeDirectory(), startPath))
        .WithSourceResolver(new SourceFileResolver(ImmutableArray<string>.Empty, Path.Combine(startPath, "scripts")))
        .WithReferences(typeof(IEntryPoint).Assembly, typeof(System.Reactive.Observer).Assembly)
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
testConfig = testConfig.WithOptimizationLevel(OptimizationLevel.Debug);
#endif

var script = CSharpScript.Create<int>("return Test1() * Y; int Test1() => 2;", testConfig, typeof(GlobalsTest));
var test3 = await script.RunAsync(new GlobalsTest());

Console.WriteLine(test3.ReturnValue);

public sealed class GlobalsTest
{
    public int Y = 10;
}