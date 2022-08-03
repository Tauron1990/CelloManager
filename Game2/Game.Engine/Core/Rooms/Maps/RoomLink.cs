namespace Game.Engine.Core.Rooms.Maps;

public enum LinkDirection
{
    North,
    NorthWest,
    NorthEast,
    South,
    SouthWest,
    SourthEast,
    West,
    East,
    Up,
    Down
}

public record struct RoomLink(string Target, string DisplayName, LinkDirection LinkDirection);