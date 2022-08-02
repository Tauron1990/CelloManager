using System.Collections.Immutable;
using Game.Engine.Packageing.Files;
using Game.Engine.Packageing.ScriptHosting;

namespace Game.Engine.Packageing;

public sealed record InternalGamePackage(
    string OriginalPath, GamePackage GamePackage, PackageScriptManager ScriptManager, PackageContentManager ContentManager);

public sealed record GamePackage(string Name, Version Version, ImmutableList<string> LoadBefore);