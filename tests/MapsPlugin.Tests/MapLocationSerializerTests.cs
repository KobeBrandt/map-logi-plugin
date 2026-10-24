namespace Loupedeck.MapsPlugin.Tests;

using System.Globalization;

using Helpers;

using Xunit;

public class MapLocationSerializerTests
{
    private static readonly MapLocation Paris = new(new GeoPoint(48.8584, 2.2945), 16);

    [Fact]
    public void Serialize_RoundTrips()
    {
        Assert.True(MapLocationSerializer.TryDeserialize(MapLocationSerializer.Serialize(Paris), out var location));
        Assert.Equal(Paris, location);
    }

    [Fact]
    public void Serialize_IgnoresCurrentCulture()
    {
        var previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("nl-BE");
        try
        {
            Assert.Equal("48.8584;2.2945;16", MapLocationSerializer.Serialize(Paris));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void TryDeserialize_ClampsOutOfRangeValues()
    {
        Assert.True(MapLocationSerializer.TryDeserialize("95;2;40", out var location));
        Assert.Equal(MapConstants.MaxLatitude, location.Center.Latitude);
        Assert.Equal(MapConstants.MaxZoom, location.Zoom);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1;2")]
    [InlineData("1;2;3;4")]
    [InlineData("x;2;3")]
    [InlineData("1;y;3")]
    [InlineData("1;2;1.5")]
    public void TryDeserialize_Invalid_ReturnsFalse(String text) =>
        Assert.False(MapLocationSerializer.TryDeserialize(text, out _));
}
