namespace Loupedeck.MapsPlugin.Helpers;

using System.Globalization;

public readonly record struct GridCell(Int32 Column, Int32 Row);

// Splits one large map into a grid of buttons. Each cell shows the map one button-width away from its neighbours.
public static class MapGrid
{
    private static readonly GridCell CenterCell = new(MapConstants.GridColumns / 2, MapConstants.GridRows / 2);
    private static readonly GridCell AttributionCell = new(MapConstants.GridColumns - 1, MapConstants.GridRows - 1);
    private static readonly GridCell ZoomInCell = CenterCell with { Row = CenterCell.Row - 1 };
    private static readonly GridCell ZoomOutCell = CenterCell with { Row = CenterCell.Row + 1 };

    private const Int32 CellCount = MapConstants.GridRows * MapConstants.GridColumns;

    public static IEnumerable<GridCell> Cells() =>
        Enumerable.Range(MapConstants.BackButtonSlots, CellCount - MapConstants.BackButtonSlots).Select(FromIndex);

    public static String ToParameter(GridCell cell) => ToIndex(cell).ToString(CultureInfo.InvariantCulture);

    public static Boolean TryParse(String parameter, out GridCell cell)
    {
        cell = default;
        if (!Int32.TryParse(parameter, NumberStyles.None, CultureInfo.InvariantCulture, out var index)
            || index < MapConstants.BackButtonSlots
            || index >= CellCount)
        {
            return false;
        }

        cell = FromIndex(index);
        return true;
    }

    public static Boolean IsCenter(GridCell cell) => cell == CenterCell;

    // Zoom mode turns the keys directly above and below the center into zoom in and zoom out.
    public static Boolean TryGetZoomStep(GridCell cell, out Int32 zoomStep)
    {
        zoomStep = cell == ZoomInCell ? MapConstants.ZoomStep : cell == ZoomOutCell ? -MapConstants.ZoomStep : 0;
        return zoomStep != 0;
    }

    public static MapOverlays OverlaysFor(GridCell cell) =>
        (IsCenter(cell) ? MapOverlays.CenterMarker : MapOverlays.None)
        | (cell == AttributionCell ? MapOverlays.Attribution : MapOverlays.None);

    public static MapLocation LocationOf(MapLocation center, GridCell cell)
    {
        var deltaX = (cell.Column - CenterCell.Column) * MapConstants.TileSize;
        var deltaY = (cell.Row - CenterCell.Row) * MapConstants.TileSize;
        var world = TileMath.ToWorld(center.Center, center.Zoom);
        var moved = TileMath.Move(world, deltaX, deltaY, center.Zoom);
        return center with { Center = TileMath.ToGeo(moved, center.Zoom) };
    }

    private static GridCell FromIndex(Int32 index) => new(index % MapConstants.GridColumns, index / MapConstants.GridColumns);

    private static Int32 ToIndex(GridCell cell) => (cell.Row * MapConstants.GridColumns) + cell.Column;
}
