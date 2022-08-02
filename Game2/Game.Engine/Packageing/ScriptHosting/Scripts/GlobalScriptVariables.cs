using Game.Engine.Packageing.Files;
using JetBrains.Annotations;

namespace Game.Engine.Packageing.ScriptHosting.Scripts;

[PublicAPI]
public sealed class GlobalScriptVariables
{
    public GlobalScriptVariables(PackageScriptManager scriptManager, PackageContentManager contentManager)
    {
        ScriptManager = scriptManager;
        ContentManager = contentManager;
    }

    public PackageScriptManager ScriptManager { get; }

    public PackageContentManager ContentManager { get; }
}