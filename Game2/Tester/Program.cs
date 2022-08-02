// See https://aka.ms/new-console-template for more information


using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

await Task.Delay(1000);

var startPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

var testConfig = ScriptOptions.Default
    .WithMetadataResolver(ScriptMetadataResolver.Default.WithSearchPaths(RuntimeEnvironment.GetRuntimeDirectory(), startPath))
        .WithSourceResolver(new SourceFileResolver(ImmutableArray<string>.Empty, Path.Combine(startPath, "scripts")))
    .AddReferences(Assembly.GetExecutingAssembly())
    .WithOptimizationLevel(OptimizationLevel.Release)
    .WithImports(
        "System",
        "System.Collections.Generic",
        "System.Linq",
        "System.Threading.Tasks");


#if DEBUG
testConfig = testConfig.WithOptimizationLevel(OptimizationLevel.Debug);
#endif

var script = CSharpScript.Create<int>("int YY = Y(); return new TestClass().Test1() * YY; class TestClass { public int Test1() => 2; }", testConfig, typeof(GlobalsTest));
var test1 = script.GetCompilation();
var test2 = script.Compile();
var test3 = await script.RunAsync(new GlobalsTest());

Console.WriteLine(test3.ReturnValue);

public sealed class GlobalsTest
{
    public static int Y() => 10;
}