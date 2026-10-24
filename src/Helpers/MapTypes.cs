namespace Loupedeck.MapsPlugin.Helpers;

public readonly record struct GeoPoint(Double Latitude, Double Longitude);

public readonly record struct WorldPoint(Double X, Double Y);

public readonly record struct MapLocation(GeoPoint Center, Int32 Zoom);

public readonly record struct TileKey(Int32 Zoom, Int32 X, Int32 Y);

public readonly record struct TilePlacement(TileKey Key, Int32 Left, Int32 Top, Int32 Size);
