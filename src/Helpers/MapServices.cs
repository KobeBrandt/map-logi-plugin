namespace Loupedeck.MapsPlugin.Helpers;

public static class MapServices
{
    public static HttpDownloader Downloader { get; } = new(MapHttpClient.Create());

    public static TileProvider Tiles { get; } = new(new TileCache(MapConstants.TileCacheCapacity), Downloader.TryGetBytesAsync, () => DateTime.UtcNow);

    public static MapRenderer Renderer { get; } = new(Tiles);

    public static MapView View { get; } = new(MapConstants.DefaultHome);

    public static PlaceGeocoder Geocoder { get; } = new(Downloader.TryGetStringAsync);

    public static MapImageInvalidator Invalidator { get; } = new(View, Tiles);
}
