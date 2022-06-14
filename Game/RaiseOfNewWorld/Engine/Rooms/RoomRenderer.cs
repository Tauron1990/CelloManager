using SystemsRx.Systems.Conventional;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Rooms;

public class RoomRenderer : IReactToEventSystem<RenderRoom>, IReactToEventSystem<GameViewEvent>
{
    private View? _view;

    public void Process(RenderRoom eventData)
    {
        if(_view == null) return;
        
        eventData.Room.Display(_view);
    }

    public void Process(GameViewEvent eventData)
        => _view = eventData.GameView;
}