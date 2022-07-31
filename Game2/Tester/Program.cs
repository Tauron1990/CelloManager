// See https://aka.ms/new-console-template for more information

using Game.Engine.Packageing;
using Game.Engine.Packageing.Files.Platform.VirtualFileSystem;
using Platform.VirtualFileSystem;

var test = FileSystem.FileSystemManager;
var dic = test.ResolveDirectory("");

Console.WriteLine("Hello, World!");