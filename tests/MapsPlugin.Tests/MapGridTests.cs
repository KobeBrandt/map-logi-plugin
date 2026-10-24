namespace Loupedeck.MapsPlugin.Tests;

using Helpers;

using Xunit;

public class MapGridTests
{
    private static readonly MapLocation Home = new(new GeoPoint(50.8466, 4.3528), 13);

    [Fact]
    public void Cells_SkipTheBackButtonSlotAndKeepRowOrder()
    {
        var cells = MapGrid.Cells().ToList();

        Assert.Equal((MapConstants.GridColumns * MapConstants.GridRows) - MapConstants.BackButtonSlots, cells.Count);
        Assert.DoesNotContain(new GridCell(0, 0), cells);
        Assert.Equal(new GridCell(1, 0), cells[0]);
        Assert.Equal(new GridCell(2, 2), cells[^1]);
    }

    [Fact]
    public void ToParameter_RoundTripsThroughTryParse()
    {
        foreach (var cell in MapGrid.Cells())
        {
            Assert.True(MapGrid.TryParse(MapGrid.ToParameter(cell), out var parsed));
            Assert.Equal(cell, parsed);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Zoom")]
    [InlineData("-1")]
    [InlineData("0")]
    [InlineData("9")]
    [InlineData(" 1")]
    public void TryParse_RejectsUnknownParameters(String parameter) =>
        Assert.False(MapGrid.TryParse(parameter, out _));

    [Fact]
    public void IsCenter_OnlyMatchesTheMiddleCell()
    {
        Assert.Single(MapGrid.Cells(), MapGrid.IsCenter);
        Assert.True(MapGrid.IsCenter(new GridCell(1, 1)));
    }

    [Theory]
    [InlineData(1, 0, MapConstants.ZoomStep)]
    [InlineData(1, 2, -MapConstants.ZoomStep)]
    public void TryGetZoomStep_KeysAboveAndBelowCenterZoom(Int32 column, Int32 row, Int32 expected)
    {
        Assert.True(MapGrid.TryGetZoomStep(new GridCell(column, row), out var zoomStep));
        Assert.Equal(expected, zoomStep);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(0, 1)]
    [InlineData(2, 2)]
    public void TryGetZoomStep_OtherKeysDoNotZoom(Int32 column, Int32 row) =>
        Assert.False(MapGrid.TryGetZoomStep(new GridCell(column, row), out _));

    [Fact]
    public void OverlaysFor_MarkerInCenterAndAttributionInBottomRight()
    {
        Assert.Equal(MapOverlays.CenterMarker, MapGrid.OverlaysFor(new GridCell(1, 1)));
        Assert.Equal(MapOverlays.Attribution, MapGrid.OverlaysFor(new GridCell(2, 2)));
        Assert.Equal(MapOverlays.None, MapGrid.OverlaysFor(new GridCell(0, 0)));
    }

    [Fact]
    public void LocationOf_CenterCell_IsTheMapCenter()
    {
        var center = MapGrid.LocationOf(Home, new GridCell(1, 1));

        Assert.Equal(Home.Center.Latitude, center.Center.Latitude, 1e-9);
        Assert.Equal(Home.Center.Longitude, center.Center.Longitude, 1e-9);
        Assert.Equal(Home.Zoom, center.Zoom);
    }

    [Fact]
    public void LocationOf_ShiftsOneTileWidthPerCell()
    {
        var east = MapGrid.LocationOf(Home, new GridCell(2, 1));
        var expected = TileMath.ToWorld(Home.Center, Home.Zoom).X + MapConstants.TileSize;

        Assert.Equal(expected, TileMath.ToWorld(east.Center, east.Zoom).X, 1e-6);
        Assert.Equal(Home.Center.Latitude, east.Center.Latitude, 1e-9);
        Assert.Equal(Home.Zoom, east.Zoom);
    }

    [Fact]
    public void LocationOf_TopRow_IsNorthOfCenter() =>
        Assert.True(MapGrid.LocationOf(Home, new GridCell(1, 0)).Center.Latitude > Home.Center.Latitude);
}
