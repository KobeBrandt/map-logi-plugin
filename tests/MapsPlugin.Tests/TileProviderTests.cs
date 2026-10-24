namespace Loupedeck.MapsPlugin.Tests;

using Helpers;

using Xunit;

public class TileProviderTests
{
    private static readonly TileKey Key = new(3, 4, 2);
    private static readonly Byte[] TileBytes = [1, 2, 3];

    private DateTime _now = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task TryGetTile_Missing_DownloadsAndRaisesTileLoaded()
    {
        var requested = new List<String>();
        var provider = this.CreateProvider(url => { requested.Add(url); return TileBytes; });
        var loaded = 0;
        provider.TileLoaded += (_, _) => loaded++;

        Assert.False(provider.TryGetTile(Key, out _));
        await provider.LoadAsync(Key);

        Assert.True(provider.TryGetTile(Key, out var tile));
        Assert.Same(TileBytes, tile);
        Assert.Equal("https://tile.openstreetmap.org/3/4/2.png", Assert.Single(requested));
        Assert.Equal(1, loaded);
    }

    [Fact]
    public async Task LoadAsync_FailedDownload_DoesNotRetryBeforeDelay()
    {
        var attempts = 0;
        var provider = this.CreateProvider(_ => { attempts++; return null; });

        await provider.LoadAsync(Key);
        await provider.LoadAsync(Key);

        Assert.Equal(1, attempts);
        Assert.False(provider.TryGetTile(Key, out _));
    }

    [Fact]
    public async Task LoadAsync_FailedDownload_RetriesAfterDelay()
    {
        var attempts = 0;
        var provider = this.CreateProvider(_ => ++attempts == 1 ? null : TileBytes);

        await provider.LoadAsync(Key);
        this._now += MapConstants.FailedTileRetryDelay;
        await provider.LoadAsync(Key);

        Assert.Equal(2, attempts);
        Assert.True(provider.TryGetTile(Key, out _));
    }

    [Fact]
    public async Task LoadAsync_FailedDownload_DoesNotRaiseTileLoaded()
    {
        var provider = this.CreateProvider(_ => null);
        var loaded = 0;
        provider.TileLoaded += (_, _) => loaded++;

        await provider.LoadAsync(Key);

        Assert.Equal(0, loaded);
    }

    [Fact]
    public void Constructor_NullDependency_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TileProvider(null, _ => Task.FromResult(TileBytes), () => this._now));
        Assert.Throws<ArgumentNullException>(() => new TileProvider(new TileCache(1), null, () => this._now));
        Assert.Throws<ArgumentNullException>(() => new TileProvider(new TileCache(1), _ => Task.FromResult(TileBytes), null));
    }

    private TileProvider CreateProvider(Func<String, Byte[]> download) =>
        new(new TileCache(MapConstants.TileCacheCapacity), url => Task.FromResult(download(url)), () => this._now);
}
