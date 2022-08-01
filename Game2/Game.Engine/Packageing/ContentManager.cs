using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Game.Engine.Packageing;

[PublicAPI]
public sealed class ContentManager
{

    private ImmutableList<IContentProvider> _providers = ImmutableList<IContentProvider>.Empty;

    public bool Registerprovider(IContentProvider provider)
        => ImmutableInterlocked.Update(ref _providers, list => list.Add(provider));

    public Stream OpenData(string name)
        => _providers.Where(p => p.CanOpen(name)).Select(cp => cp.Open(name)).First();
}