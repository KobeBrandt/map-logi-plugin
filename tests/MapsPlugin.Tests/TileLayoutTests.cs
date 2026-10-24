namespace Loupedeck.MapsPlugin.Tests;

using Helpers;

using Xunit;

public class TileLayoutTests
{
    private const Int32 ImageSize = 90;

    [Fact]
    public void Compute_AtZoomZero_UsesTheSingleWorldTile()
    {
        var placements = TileLayout.Compute(new MapLocation(new GeoPoint(0, 0), 0), ImageSize);

        var placement = Assert.Single(placements);
        Assert.Equal(new TilePlacement(new TileKey(0, 0, 0), 0, 0, ImageSize), placement);
    }

    [Fact]
    public void Compute_OffCenter_CoversTheImageWithFourTiles()
    {
        var placements = TileLayout.Compute(new MapLocation(new GeoPoint(50.8466, 4.3528), 13), ImageSize);

        Assert.Equal(4, placements.Count);
        Assert.Contains(placements, p => p.Left <= 0 && p.Top <= 0);
        Assert.Contains(placements, p => p.Left + p.Size >= ImageSize && p.Top + p.Size >= ImageSize);
    }

    [Fact]
    public void Compute_LeavesNoGapsBetweenNeighbouringTiles()
    {
        var placements = TileLayout.Compute(new MapLocation(new GeoPoint(50.8466, 4.3528), 13), ImageSize);

        var columns = placements.Select(p => (p.Left, Right: p.Left + p.Size)).Distinct().OrderBy(c => c.Left).ToList();
        Assert.Equal(columns[0].Right, columns[1].Left);
    }

    [Fact]
    public void Compute_SkipsTilesBeyondThePoles()
    {
        var placements = TileLayout.Compute(new MapLocation(new GeoPoint(MapConstants.MaxLatitude, 0), 1), ImageSize);

        Assert.All(placements, p => Assert.InRange(p.Key.Y, 0, 1));
        Assert.Equal(2, placements.Count);
    }

    [Fact]
    public void Compute_WrapsTilesAcrossTheAntimeridian()
    {
        var placements = TileLayout.Compute(new MapLocation(new GeoPoint(0, 179.99), 2), ImageSize);

        Assert.Contains(placements, p => p.Key.X == 0);
        Assert.Contains(placements, p => p.Key.X == 3);
    }
}
