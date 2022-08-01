using System.Collections.Immutable;

namespace Game.Engine.Packageing;

public sealed record InternalGamePackage(string OriginalPath, GamePackage GamePackage);

public sealed record GamePackage(string Name, Version Version, ImmutableList<string> LoadBefore);