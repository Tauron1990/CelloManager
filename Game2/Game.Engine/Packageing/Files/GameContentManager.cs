using System.Collections.Immutable;

namespace Game.Engine.Packageing.Files;

public sealed class GameContentManager
{
    private EcsRx.MicroRx.ImmutableList<PackageContentManager> _contentManagers = EcsRx.MicroRx.ImmutableList<PackageContentManager>.Empty;

    public async ValueTask<PackageContentManager> Register(PackageContentManager gameContentManager)
    {
        await gameContentManager.Init();
        ImmutableInterlocked.Update(ref _contentManagers, (l, m) => l.Add(m), gameContentManager);

        return gameContentManager;
    }
}