namespace Loupedeck.MapsPlugin.Helpers;

// Draws white control symbols over a shaded map image.
public static class SymbolRenderer
{
    public static void DrawArrow(BitmapBuilder builder, PanDirection direction)
    {
        var (directionX, directionY) = PanDirections.ToVector(direction);
        var (centerX, centerY, length) = ShadeAndMeasure(builder);
        var head = Math.Min(builder.Width, builder.Height) * MapConstants.ArrowHeadRatio;

        var tipX = centerX + (directionX * length);
        var tipY = centerY + (directionY * length);
        DrawLine(builder, centerX - (directionX * length), centerY - (directionY * length), tipX, tipY);

        // The two head strokes run back from the tip, splayed sideways along the perpendicular (-y, x).
        var backX = tipX - (directionX * head);
        var backY = tipY - (directionY * head);
        DrawLine(builder, tipX, tipY, backX - (directionY * head), backY + (directionX * head));
        DrawLine(builder, tipX, tipY, backX + (directionY * head), backY - (directionX * head));
    }

    public static void DrawZoomSign(BitmapBuilder builder, Int32 zoomStep)
    {
        var (centerX, centerY, length) = ShadeAndMeasure(builder);
        DrawLine(builder, centerX - length, centerY, centerX + length, centerY);
        if (zoomStep > 0)
        {
            DrawLine(builder, centerX, centerY - length, centerX, centerY + length);
        }
    }

    private static (Single CenterX, Single CenterY, Single Length) ShadeAndMeasure(BitmapBuilder builder)
    {
        builder.FillRectangle(0, 0, builder.Width, builder.Height, MapConstants.SymbolShadeColor);
        var length = Math.Min(builder.Width, builder.Height) * MapConstants.SymbolLengthRatio;
        return (builder.Width / 2f, builder.Height / 2f, length);
    }

    private static void DrawLine(BitmapBuilder builder, Single fromX, Single fromY, Single toX, Single toY) =>
        builder.DrawLine(fromX, fromY, toX, toY, MapConstants.SymbolColor, MapConstants.SymbolLineWidth);
}
