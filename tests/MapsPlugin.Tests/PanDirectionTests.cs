namespace Loupedeck.MapsPlugin.Tests;

using Helpers;

using Xunit;

public class PanDirectionTests
{
    [Theory]
    [InlineData(PanDirection.North, 0, -1)]
    [InlineData(PanDirection.East, 1, 0)]
    [InlineData(PanDirection.South, 0, 1)]
    [InlineData(PanDirection.West, -1, 0)]
    public void ToVector_PointsInScreenDirection(PanDirection direction, Int32 x, Int32 y) =>
        Assert.Equal((x, y), PanDirections.ToVector(direction));

    [Fact]
    public void ToVector_UnknownDirection_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => PanDirections.ToVector((PanDirection)42));

    [Theory]
    [InlineData("North", true)]
    [InlineData("west", true)]
    [InlineData("Up", false)]
    [InlineData("42", false)]
    [InlineData(null, false)]
    public void TryParse_AcceptsOnlyKnownDirections(String name, Boolean expected) =>
        Assert.Equal(expected, PanDirections.TryParse(name, out _));
}
