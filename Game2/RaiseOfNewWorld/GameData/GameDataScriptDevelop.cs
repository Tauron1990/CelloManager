using System.Reactive;
using System.Reactive.Linq;
using EcsRx.Extensions;
using Game.Engine;
using Game.Engine.Core;
using Game.Engine.Core.Movement;
using Game.Engine.Core.Rooms;
using Game.Engine.Core.Rooms.Types;
using Game.Engine.Packageing.ScriptHosting.Scripts;
using Game.Engine.Screens;
using Terminal.Gui;

namespace RaiseOfNewWorld.GameData;

public class GameDataScriptDevelop : GlobalScriptVariables
{
    public GameDataScriptDevelop()
        : base(null!, null!, null!)
    {
    }

    public Unit Run()
    {
        CreateDimesion(
            db => db.WithDimension(
                2,
                builder =>
                {
                    //builder.WithRoom("chapter1", b => b.WithFactory((_, manager) => ))
                }));


        return Unit.Default;
    }
}