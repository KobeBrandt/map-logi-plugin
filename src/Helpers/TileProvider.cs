namespace Loupedeck.MapsPlugin.Helpers;

using System.Collections.Concurrent;
using System.Globalization;

// Serves tiles from the cache and fetches missing ones in the background, because button images are drawn synchronously.
public sealed class TileProvider
{
    private readonly TileCache _cache;
    private readonly Func<String, Task<Byte[]>> _download;
    private readonly Func<DateTime> _clock;
    private readonly ConcurrentDictionary<TileKey, Lazy<Task>> _pending = new();
    private readonly ConcurrentDictionary<TileKey, DateTime> _failedAt = new();

    public TileProvider(TileCache cache, Func<String, Task<Byte[]>> download, Func<DateTime> clock)
    {
        this._cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this._download = download ?? throw new ArgumentNullException(nameof(download));
        this._clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public event EventHandler TileLoaded;

    public static String GetTileUrl(TileKey key) =>
        String.Format(CultureInfo.InvariantCulture, MapConstants.TileUrlFormat, key.Zoom, key.X, key.Y);

    public Boolean TryGetTile(TileKey key, out Byte[] tile)
    {
        if (this._cache.TryGet(key, out tile))
        {
            return true;
        }

        _ = this.LoadAsync(key);
        return false;
    }

    public Task LoadAsync(TileKey key)
    {
        if (this.IsWaitingToRetry(key))
        {
            return Task.CompletedTask;
        }

        // Lazy starts the download only after it is stored, so a fast download cannot remove its entry before it exists.
        return this._pending.GetOrAdd(key, _ => new Lazy<Task>(() => Task.Run(() => this.DownloadAsync(key)))).Value;
    }

    // A failed tile is not retried on every redraw, which would hammer the tile server while offline.
    private Boolean IsWaitingToRetry(TileKey key) =>
        this._failedAt.TryGetValue(key, out var failedAt) && this._clock() - failedAt < MapConstants.FailedTileRetryDelay;

    private async Task DownloadAsync(TileKey key)
    {
        try
        {
            var tile = await this._download(GetTileUrl(key));
            this.StoreResult(key, tile);
        }
        finally
        {
            this._pending.TryRemove(key, out _);
        }
    }

    private void StoreResult(TileKey key, Byte[] tile)
    {
        if (tile == null)
        {
            this._failedAt[key] = this._clock();
            return;
        }

        this._failedAt.TryRemove(key, out _);
        this._cache.Add(key, tile);
        this.TileLoaded?.Invoke(this, EventArgs.Empty);
    }
}
