namespace Loupedeck.MapsPlugin.Tests;

using Helpers;

using Xunit;

public class TileCacheTests
{
    private static readonly TileKey First = new(1, 0, 0);
    private static readonly TileKey Second = new(1, 1, 0);
    private static readonly TileKey Third = new(1, 0, 1);

    [Fact]
    public void TryGet_ReturnsAddedTile()
    {
        var cache = new TileCache(2);
        var tile = new Byte[] { 1, 2, 3 };
        cache.Add(First, tile);

        Assert.True(cache.TryGet(First, out var found));
        Assert.Same(tile, found);
    }

    [Fact]
    public void TryGet_MissingTile_ReturnsFalse()
    {
        var cache = new TileCache(2);

        Assert.False(cache.TryGet(First, out var found));
        Assert.Null(found);
    }

    [Fact]
    public void Add_OverCapacity_EvictsLeastRecentlyUsed()
    {
        var cache = new TileCache(2);
        cache.Add(First, [1]);
        cache.Add(Second, [2]);
        cache.TryGet(First, out _);

        cache.Add(Third, [3]);

        Assert.True(cache.TryGet(First, out _));
        Assert.False(cache.TryGet(Second, out _));
        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void Add_SameKey_ReplacesTile()
    {
        var cache = new TileCache(2);
        cache.Add(First, [1]);

        cache.Add(First, [9]);

        cache.TryGet(First, out var found);
        Assert.Equal(new Byte[] { 9 }, found);
        Assert.Equal(1, cache.Count);
    }

    [Fact]
    public void Add_NullTile_Throws() => Assert.Throws<ArgumentNullException>(() => new TileCache(1).Add(First, null));

    [Fact]
    public void Constructor_NonPositiveCapacity_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => new TileCache(0));
}
