namespace Loupedeck.MapsPlugin.Helpers;

// Web Mercator projection as used by OpenStreetMap slippy map tiles.
public static class TileMath
{
    public static Double WorldSize(Int32 zoom) => MapConstants.TileSize * Math.Pow(2, zoom);

    public static Int32 TileCount(Int32 zoom) => 1 << zoom;

    public static Int32 ClampZoom(Int32 zoom) => Math.Clamp(zoom, MapConstants.MinZoom, MapConstants.MaxZoom);

    public static Double ClampLatitude(Double latitude) => Math.Clamp(latitude, -MapConstants.MaxLatitude, MapConstants.MaxLatitude);

    public static WorldPoint ToWorld(GeoPoint point, Int32 zoom)
    {
        var worldSize = WorldSize(zoom);
        var latitudeRadians = ClampLatitude(point.Latitude) * Math.PI / MapConstants.HalfLongitudeSpan;
        var x = (point.Longitude + MapConstants.HalfLongitudeSpan) / MapConstants.FullLongitudeSpan * worldSize;
        var mercatorY = Math.Log(Math.Tan(latitudeRadians) + (1 / Math.Cos(latitudeRadians)));
        var y = (1 - (mercatorY / Math.PI)) / 2 * worldSize;
        return new WorldPoint(x, y);
    }

    public static GeoPoint ToGeo(WorldPoint point, Int32 zoom)
    {
        var worldSize = WorldSize(zoom);
        var longitude = (point.X / worldSize * MapConstants.FullLongitudeSpan) - MapConstants.HalfLongitudeSpan;
        var mercatorY = Math.PI - (2 * Math.PI * point.Y / worldSize);
        var latitude = Math.Atan(Math.Sinh(mercatorY)) * MapConstants.HalfLongitudeSpan / Math.PI;
        return new GeoPoint(ClampLatitude(latitude), longitude);
    }

    public static WorldPoint Move(WorldPoint point, Double deltaX, Double deltaY, Int32 zoom)
    {
        var worldSize = WorldSize(zoom);
        var x = WrapPositive(point.X + deltaX, worldSize);
        var y = Math.Clamp(point.Y + deltaY, 0, worldSize);
        return new WorldPoint(x, y);
    }

    public static Int32 WrapTileX(Int32 tileX, Int32 zoom) => (Int32)WrapPositive(tileX, TileCount(zoom));

    // Picks the deepest zoom whose single-tile view still covers the given span in degrees.
    public static Int32 ZoomToFit(Double spanDegrees)
    {
        if (spanDegrees <= 0 || Double.IsNaN(spanDegrees))
        {
            return MapConstants.MaxZoom;
        }

        return ClampZoom((Int32)Math.Floor(Math.Log2(MapConstants.FullLongitudeSpan / spanDegrees)));
    }

    private static Double WrapPositive(Double value, Double modulus) => ((value % modulus) + modulus) % modulus;
}
