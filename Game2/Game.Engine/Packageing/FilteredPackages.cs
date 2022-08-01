using System.Collections.Immutable;

namespace Game.Engine.Packageing;

public sealed record FilteredPackages(ImmutableList<string> ExcludePackages);