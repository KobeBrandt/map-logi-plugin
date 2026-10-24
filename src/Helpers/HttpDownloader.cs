namespace Loupedeck.MapsPlugin.Helpers;

// Downloads a URL and reports every failure as null after logging it, so callers only deal with "got it" or "did not".
// Failures: network errors (HttpRequestException), timeouts (TaskCanceledException) and non-success status codes.
public sealed class HttpDownloader
{
    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _connections = new(MapConstants.MaxConcurrentDownloads);

    public HttpDownloader(HttpClient httpClient) => this._httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    public async Task<Byte[]> TryGetBytesAsync(String url)
    {
        await this._connections.WaitAsync();
        try
        {
            return await this.GetBytesOrNullAsync(url);
        }
        finally
        {
            this._connections.Release();
        }
    }

    public async Task<String> TryGetStringAsync(String url)
    {
        var bytes = await this.TryGetBytesAsync(url);
        return bytes == null ? null : System.Text.Encoding.UTF8.GetString(bytes);
    }

    private async Task<Byte[]> GetBytesOrNullAsync(String url)
    {
        try
        {
            using var response = await this._httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                PluginLog.Warning($"Request to {url} failed with status {(Int32)response.StatusCode}");
                return null;
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            PluginLog.Warning(ex, $"Request to {url} failed");
            return null;
        }
    }
}
