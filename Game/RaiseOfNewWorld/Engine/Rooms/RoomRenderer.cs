using SystemsRx.Systems.Conventional;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Rooms;

public class RoomRenderer : IReactToEventSystem<RenderRoom>
{
    private readonly GameManager _gameManager;
    public static View? View { get; set; }

    public RoomRenderer(GameManager gameManager) => _gameManager = gameManager;

    public void Process(RenderRoom eventData)
    {
        if(View is null) return;

        Application.MainLoop.Invoke(() =>
        {
            View.RemoveAll();
            eventData.Room.Display(View, _gameManager);
        });
    }
}