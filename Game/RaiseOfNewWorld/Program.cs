using RaiseOfNewWorld.Engine.Data;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;
using RaiseOfNewWorld.Screens;
using Terminal.Gui;

var testParser = new TextParser(File.ReadAllText("test.txt"));
var result = testParser.Parse(ContentManager.Empty);

result.Validate();

Console.WriteLine(result);
Console.ReadKey();

Console.Title = "Rise of New World";

Console.WriteLine("Starte Spiel");
Application.Run<ScreenManager>();
Console.WriteLine("Beende Spiel");
Application.Shutdown();