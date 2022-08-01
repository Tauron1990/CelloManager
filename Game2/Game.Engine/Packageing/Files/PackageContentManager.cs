using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Game.Engine.Packageing.Files;

[PublicAPI]
public sealed class PackageContentManager
{
    private ImmutableList<IContentProvider> _providers = ImmutableList<IContentProvider>.Empty;

    public bool Registerprovider(IContentProvider provider)
        => ImmutableInterlocked.Update(ref _providers, (list, prov) => list.Add(prov), provider);

    public Stream OpenData(string name)
        => _providers.Where(p => p.CanOpen(name)).Select(cp => cp.Open(name)).First();

    public PackageContentManager(string rootDirectory) => Registerprovider(new DataFileContentProvider(rootDirectory));
}