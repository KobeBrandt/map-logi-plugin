namespace Loupedeck.MapsPlugin.Helpers;

public static class MapHttpClient
{
    public static HttpClient Create(HttpMessageHandler handler = null)
    {
        var httpClient = handler == null ? new HttpClient() : new HttpClient(handler);
        httpClient.Timeout = MapConstants.RequestTimeout;
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(MapConstants.UserAgent);
        return httpClient;
    }
}
