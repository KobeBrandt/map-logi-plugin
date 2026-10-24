namespace Loupedeck.MapsPlugin.Helpers;

// Draws the map for a location onto a button image. Tiles still downloading show as background until they arrive.
public sealed class MapRenderer
{
    private readonly TileProvider _tiles;

    public MapRenderer(TileProvider tiles) => this._tiles = tiles ?? throw new ArgumentNullException(nameof(tiles));

    public BitmapImage Render(MapLocation location, PluginImageSize imageSize, MapOverlays overlays = MapOverlays.All)
    {
        using var builder = new BitmapBuilder(imageSize);
        this.Draw(builder, location, overlays);
        return builder.ToImage();
    }

    public void Draw(BitmapBuilder builder, MapLocation location, MapOverlays overlays = MapOverlays.All)
    {
        builder.Clear(MapConstants.BackgroundColor);
        this.DrawTiles(builder, location);
        if (overlays.HasFlag(MapOverlays.CenterMarker))
        {
            DrawCenterMarker(builder);
        }

        if (overlays.HasFlag(MapOverlays.Attribution))
        {
            DrawAttribution(builder);
        }
    }

    private void DrawTiles(BitmapBuilder builder, MapLocation location)
    {
        foreach (var placement in TileLayout.Compute(location, builder.Width))
        {
            if (this._tiles.TryGetTile(placement.Key, out var tile))
            {
                builder.DrawImage(tile, placement.Left, placement.Top, placement.Size, placement.Size);
            }
        }
    }

    private static void DrawCenterMarker(BitmapBuilder builder)
    {
        var centerX = builder.Width / 2f;
        var centerY = builder.Height / 2f;
        builder.FillCircle(centerX, centerY, MapConstants.MarkerRadius + MapConstants.MarkerBorder, MapConstants.MarkerBorderColor);
        builder.FillCircle(centerX, centerY, MapConstants.MarkerRadius, MapConstants.MarkerColor);
    }

    // The OpenStreetMap licence requires visible attribution wherever its tiles are shown.
    private static void DrawAttribution(BitmapBuilder builder)
    {
        var top = builder.Height - MapConstants.AttributionHeight;
        builder.FillRectangle(0, top, builder.Width, MapConstants.AttributionHeight, MapConstants.AttributionBackground);
        builder.DrawText(
            MapConstants.Attribution,
            0,
            top,
            builder.Width,
            MapConstants.AttributionHeight,
            MapConstants.AttributionColor,
            MapConstants.AttributionFontSize,
            MapConstants.AttributionHeight,
            0,
            null);
    }
}
