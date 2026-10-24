namespace Loupedeck.MapsPlugin.Helpers;

// Works out which tiles cover a square image centered on a location, and where each tile lands in that image.
public static class TileLayout
{
    public static IReadOnlyList<TilePlacement> Compute(MapLocation location, Int32 imageSize)
    {
        var center = TileMath.ToWorld(location.Center, location.Zoom);
        var halfView = MapConstants.TileSize / 2.0;
        var left = center.X - halfView;
        var top = center.Y - halfView;
        var scale = (Double)imageSize / MapConstants.TileSize;

        var placements = new List<TilePlacement>();
        foreach (var tileY in TileRange(top))
        {
            foreach (var tileX in TileRange(left))
            {
                AddPlacement(placements, location.Zoom, tileX, tileY, left, top, scale);
            }
        }

        return placements;
    }

    private static IEnumerable<Int32> TileRange(Double viewStart)
    {
        var first = (Int32)Math.Floor(viewStart / MapConstants.TileSize);
        var last = (Int32)Math.Floor((viewStart + MapConstants.TileSize - 1) / MapConstants.TileSize);
        return Enumerable.Range(first, last - first + 1);
    }

    // Tiles above the north pole or below the south pole do not exist; the background shows there instead.
    private static void AddPlacement(List<TilePlacement> placements, Int32 zoom, Int32 tileX, Int32 tileY, Double left, Double top, Double scale)
    {
        if (tileY < 0 || tileY >= TileMath.TileCount(zoom))
        {
            return;
        }

        var destinationLeft = ToImagePixels((tileX * MapConstants.TileSize) - left, scale);
        var destinationTop = ToImagePixels((tileY * MapConstants.TileSize) - top, scale);
        var destinationRight = ToImagePixels(((tileX + 1) * MapConstants.TileSize) - left, scale);
        var key = new TileKey(zoom, TileMath.WrapTileX(tileX, zoom), tileY);
        placements.Add(new TilePlacement(key, destinationLeft, destinationTop, destinationRight - destinationLeft));
    }

    private static Int32 ToImagePixels(Double worldPixels, Double scale) => (Int32)Math.Round(worldPixels * scale);
}
