using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Game.Engine.Packageing.Files;

[PublicAPI]
public sealed class PackageContentManager
{
    private ImmutableList<IContentProvider> _providers = ImmutableList<IContentProvider>.Empty;

    public PackageContentManager(string rootDirectory)
    {
        Registerprovider(new DataFileContentProvider(rootDirectory));
    }

    public bool Registerprovider(IContentProvider provider)
    {
        return ImmutableInterlocked.Update(ref _providers, (list, prov) => list.Add(prov), provider);
    }

    public async ValueTask Init()
    {
        await Task.WhenAll(_providers.Select(p => p.Init()));
    }

    public bool CanOpen(string name)
    {
        return _providers.Any(p => p.CanOpen(name));
    }

    public Stream OpenData(string name)
    {
        return _providers.Where(p => p.CanOpen(name)).Select(cp => cp.Open(name)).First();
    }
}