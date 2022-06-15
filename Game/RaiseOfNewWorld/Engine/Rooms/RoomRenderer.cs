using SystemsRx.Systems.Conventional;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Rooms;

public class RoomRenderer : IReactToEventSystem<RenderRoom>, IReactToEventSystem<GameViewEvent>
{
    private readonly GameManager _gameManager;
    private View? _view;

    public RoomRenderer(GameManager gameManager) => _gameManager = gameManager;

    public void Process(RenderRoom eventData)
    {
        if(_view is null) return;

        Application.MainLoop.Invoke(() =>
        {
            _view.RemoveAll();
            eventData.Room.Display(_view, _gameManager);
        });
    }

    public void Process(GameViewEvent eventData)
        => _view = eventData.GameView;
}