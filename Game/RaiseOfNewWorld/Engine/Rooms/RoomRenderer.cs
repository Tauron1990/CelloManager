using SystemsRx.Systems.Conventional;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Rooms;

public class RoomRenderer : IReactToEventSystem<RenderRoom>
{
    private readonly GameManager _gameManager;

    private RoomBase? _currentRoom;

    public RoomRenderer(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public static View? View { get; set; }

    public void Process(RenderRoom eventData)
    {
        if (View is null) return;

        Application.MainLoop.Invoke(
            () =>
            {
                _currentRoom?.Close();
                View.RemoveAll();
                eventData.Room.Display(
                    View,
                    _gameManager);
                _currentRoom = eventData.Room;
            });
    }
}