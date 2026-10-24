namespace Loupedeck.MapsPlugin.Tests;

using Helpers;

using Xunit;

public class PlaceGeocoderTests
{
    private const String ParisResponse =
        """[{"lat":"48.8582599","lon":"2.2945006","boundingbox":["48.8574753","48.8590453","2.2933119","2.2956897"]}]""";

    [Fact]
    public async Task FindAsync_Match_ReturnsLocationZoomedToBoundingBox()
    {
        String requestedUrl = null;
        var geocoder = new PlaceGeocoder(url => { requestedUrl = url; return Task.FromResult(ParisResponse); });

        var location = await geocoder.FindAsync(" Eiffel Tower ");

        Assert.Equal(48.8582599, location.Value.Center.Latitude, 1e-7);
        Assert.Equal(2.2945006, location.Value.Center.Longitude, 1e-7);
        Assert.Equal(17, location.Value.Zoom);
        Assert.EndsWith("&q=Eiffel%20Tower", requestedUrl);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task FindAsync_EmptyQuery_ReturnsNullWithoutRequest(String query)
    {
        var requests = 0;
        var geocoder = new PlaceGeocoder(_ => { requests++; return Task.FromResult(ParisResponse); });

        Assert.Null(await geocoder.FindAsync(query));
        Assert.Equal(0, requests);
    }

    [Fact]
    public async Task FindAsync_DownloadFailed_ReturnsNull()
    {
        var geocoder = new PlaceGeocoder(_ => Task.FromResult<String>(null));

        Assert.Null(await geocoder.FindAsync("Paris"));
    }

    [Fact]
    public async Task FindAsync_NoMatch_ReturnsNull()
    {
        var geocoder = new PlaceGeocoder(_ => Task.FromResult("[]"));

        Assert.Null(await geocoder.FindAsync("Nowhere at all"));
    }

    [Fact]
    public void TryParseFirstResult_MissingBoundingBox_UsesDefaultZoom()
    {
        Assert.True(PlaceGeocoder.TryParseFirstResult("""[{"lat":"1","lon":"2"}]""", out var location));
        Assert.Equal(MapConstants.DefaultHome.Zoom, location.Zoom);
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("{}")]
    [InlineData("""[{"lon":"2"}]""")]
    [InlineData("""[{"lat":1,"lon":2}]""")]
    [InlineData("""[{"lat":"north","lon":"2"}]""")]
    [InlineData("""[{"lat":null,"lon":"2"}]""")]
    [InlineData("""[{"lat":"1","lon":"2","boundingbox":"wide"}]""")]
    public void TryParseFirstResult_MalformedResponse_ReturnsFalse(String json) =>
        Assert.False(PlaceGeocoder.TryParseFirstResult(json, out _));

    [Fact]
    public void Constructor_NullDownload_Throws() => Assert.Throws<ArgumentNullException>(() => new PlaceGeocoder(null));
}
