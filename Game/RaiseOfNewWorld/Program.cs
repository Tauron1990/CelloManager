// See https://aka.ms/new-console-template for more information

using EcsRx.ReactiveData;
using Newtonsoft.Json;
using RaiseOfNewWorld.Screens;
using Terminal.Gui;

Console.Title = "Rise of New World";

Console.WriteLine("Starte Spiel");
Application.Run<ScreenManager>();
Console.WriteLine("Beende Spiel");
Application.Shutdown();