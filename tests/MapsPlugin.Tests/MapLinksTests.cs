namespace Loupedeck.MapsPlugin.Tests;

using Helpers;

using Xunit;

public class MapLinksTests
{
    [Fact]
    public void ForLocation_BuildsOpenStreetMapUrl()
    {
        var url = MapLinks.ForLocation(new MapLocation(new GeoPoint(48.858412345, 2.2945), 16));

        Assert.Equal("https://www.openstreetmap.org/?mlat=48.85841&mlon=2.2945#map=16/48.85841/2.2945", url);
    }

    [Fact]
    public void OpenInBrowser_EmptyUrl_ReturnsFalse() => Assert.False(MapLinks.OpenInBrowser(String.Empty));
}
