namespace Loupedeck.MapsPlugin.Tests;

using Helpers;

using Xunit;

public class MapImageInvalidatorTests
{
    [Fact]
    public async Task Invalidated_RaisedForViewChangesAndLoadedTiles()
    {
        var view = new MapView(MapConstants.DefaultHome);
        var tiles = new TileProvider(new TileCache(1), _ => Task.FromResult(new Byte[] { 1 }), () => DateTime.UtcNow);
        var invalidator = new MapImageInvalidator(view, tiles);
        var raised = 0;
        invalidator.Invalidated += (_, _) => raised++;

        view.ZoomBy(1);
        await tiles.LoadAsync(new TileKey(0, 0, 0));

        Assert.Equal(2, raised);
    }

    [Fact]
    public void Constructor_NullDependency_Throws()
    {
        var tiles = new TileProvider(new TileCache(1), _ => Task.FromResult<Byte[]>(null), () => DateTime.UtcNow);
        Assert.Throws<ArgumentNullException>(() => new MapImageInvalidator(null, tiles));
        Assert.Throws<ArgumentNullException>(() => new MapImageInvalidator(new MapView(MapConstants.DefaultHome), null));
    }
}
