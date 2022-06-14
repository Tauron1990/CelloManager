using EcsRx.Components;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Rooms;

public abstract class RoomBase : IComponent
{
    public abstract void Display(View view);
}