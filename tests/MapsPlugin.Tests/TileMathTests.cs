namespace Loupedeck.MapsPlugin.Tests;

using Helpers;

using Xunit;

public class TileMathTests
{
    private const Double Precision = 1e-6;

    [Fact]
    public void ToWorld_MapsNullIslandToWorldCenter()
    {
        var world = TileMath.ToWorld(new GeoPoint(0, 0), 2);

        Assert.Equal(512, world.X, Precision);
        Assert.Equal(512, world.Y, Precision);
    }

    [Fact]
    public void ToGeo_IsInverseOfToWorld()
    {
        var brussels = new GeoPoint(50.8466, 4.3528);

        var roundTrip = TileMath.ToGeo(TileMath.ToWorld(brussels, 13), 13);

        Assert.Equal(brussels.Latitude, roundTrip.Latitude, Precision);
        Assert.Equal(brussels.Longitude, roundTrip.Longitude, Precision);
    }

    [Fact]
    public void ToWorld_ClampsPolarLatitudes()
    {
        var world = TileMath.ToWorld(new GeoPoint(90, 0), 0);

        Assert.Equal(0, world.Y, 1e-3);
    }

    [Fact]
    public void Move_WrapsAroundTheAntimeridian()
    {
        var moved = TileMath.Move(new WorldPoint(10, 100), -20, 0, 0);

        Assert.Equal(246, moved.X, Precision);
    }

    [Fact]
    public void Move_ClampsVerticallyToTheWorldEdge()
    {
        var moved = TileMath.Move(new WorldPoint(10, 100), 0, 1000, 0);

        Assert.Equal(256, moved.Y, Precision);
    }

    [Theory]
    [InlineData(-1, 2, 3)]
    [InlineData(4, 2, 0)]
    [InlineData(2, 2, 2)]
    public void WrapTileX_WrapsIntoTileRange(Int32 tileX, Int32 zoom, Int32 expected) =>
        Assert.Equal(expected, TileMath.WrapTileX(tileX, zoom));

    [Theory]
    [InlineData(360, 0)]
    [InlineData(0.01, 15)]
    [InlineData(0, MapConstants.MaxZoom)]
    [InlineData(-5, MapConstants.MaxZoom)]
    [InlineData(Double.NaN, MapConstants.MaxZoom)]
    [InlineData(1e-9, MapConstants.MaxZoom)]
    [InlineData(10000, MapConstants.MinZoom)]
    public void ZoomToFit_PicksZoomForSpan(Double span, Int32 expected) => Assert.Equal(expected, TileMath.ZoomToFit(span));
}
