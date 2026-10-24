namespace Loupedeck.MapsPlugin.Helpers;

public enum PanDirection
{
    North,
    East,
    South,
    West,
}

public static class PanDirections
{
    // Unit vector in screen space: x grows to the east, y grows to the south.
    public static (Int32 X, Int32 Y) ToVector(PanDirection direction) => direction switch
    {
        PanDirection.North => (0, -1),
        PanDirection.East => (1, 0),
        PanDirection.South => (0, 1),
        PanDirection.West => (-1, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown pan direction"),
    };

    public static Boolean TryParse(String name, out PanDirection direction) =>
        Enum.TryParse(name, ignoreCase: true, out direction) && Enum.IsDefined(direction);
}
