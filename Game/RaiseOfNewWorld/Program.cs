using RaiseOfNewWorld.Engine.Data.TextProcessing;
using RaiseOfNewWorld.Screens;
using Terminal.Gui;

ReadOnlySpan<char> testString = "{hallo}";
Token token;

do
{
    token = Tokenizer.NextToken(testString);
    testString = testString[token.Text.Length..];

    var tokenDebug = token.ToString();
    var stringDebug = testString.ToString();
} while (!token.IsEof);

Console.Title = "Rise of New World";

Console.WriteLine("Starte Spiel");
Application.Run<ScreenManager>();
Console.WriteLine("Beende Spiel");
Application.Shutdown();