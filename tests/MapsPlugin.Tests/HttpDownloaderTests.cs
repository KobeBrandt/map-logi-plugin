namespace Loupedeck.MapsPlugin.Tests;

using System.Net;

using Fakes;

using Helpers;

using Xunit;

public class HttpDownloaderTests
{
    private const String Url = "http://maps.test/tile.png";

    [Fact]
    public async Task TryGetBytesAsync_Success_ReturnsBody()
    {
        var downloader = new HttpDownloader(FakeHttpMessageHandler.Returning(HttpStatusCode.OK, "tile").CreateClient());

        var bytes = await downloader.TryGetBytesAsync(Url);

        Assert.Equal("tile"u8.ToArray(), bytes);
    }

    [Fact]
    public async Task TryGetStringAsync_Success_ReturnsText()
    {
        var downloader = new HttpDownloader(FakeHttpMessageHandler.Returning(HttpStatusCode.OK, "[]").CreateClient());

        Assert.Equal("[]", await downloader.TryGetStringAsync(Url));
    }

    [Fact]
    public async Task TryGetBytesAsync_ErrorStatus_ReturnsNull()
    {
        var downloader = new HttpDownloader(FakeHttpMessageHandler.Returning(HttpStatusCode.Forbidden).CreateClient());

        Assert.Null(await downloader.TryGetBytesAsync(Url));
    }

    [Fact]
    public async Task TryGetStringAsync_ErrorStatus_ReturnsNull()
    {
        var downloader = new HttpDownloader(FakeHttpMessageHandler.Returning(HttpStatusCode.NotFound).CreateClient());

        Assert.Null(await downloader.TryGetStringAsync(Url));
    }

    [Fact]
    public async Task TryGetBytesAsync_NetworkError_ReturnsNull()
    {
        var downloader = new HttpDownloader(FakeHttpMessageHandler.Throwing(new HttpRequestException("offline")).CreateClient());

        Assert.Null(await downloader.TryGetBytesAsync(Url));
    }

    [Fact]
    public async Task TryGetBytesAsync_Timeout_ReturnsNull()
    {
        var downloader = new HttpDownloader(FakeHttpMessageHandler.Throwing(new TaskCanceledException("timeout")).CreateClient());

        Assert.Null(await downloader.TryGetBytesAsync(Url));
    }

    [Fact]
    public void Constructor_NullClient_Throws() => Assert.Throws<ArgumentNullException>(() => new HttpDownloader(null));

    [Fact]
    public void MapHttpClient_SendsIdentifyingUserAgent()
    {
        using var client = MapHttpClient.Create(FakeHttpMessageHandler.Returning(HttpStatusCode.OK));

        Assert.Equal(MapConstants.UserAgent, client.DefaultRequestHeaders.UserAgent.ToString());
        Assert.Equal(MapConstants.RequestTimeout, client.Timeout);
    }
}
