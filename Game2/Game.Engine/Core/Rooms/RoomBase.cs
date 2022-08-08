using EcsRx.Components;
using Game.Engine.Packageing;
using JetBrains.Annotations;
using Terminal.Gui;

namespace Game.Engine.Core.Rooms;

[PublicAPI]
public abstract class RoomBase : IComponent
{
    public GameDataManager DataManager => GameManager.DataManager;
    
    public abstract void Display(View view, GameManager gameManager);

    public virtual void Close()
    {
    }
}