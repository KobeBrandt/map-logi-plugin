namespace Loupedeck.MapsPlugin.Helpers;

using System.Globalization;
using System.Text.Json;

// Looks up a place name with the OpenStreetMap Nominatim search API.
// Returns null when the download fails, nothing matches, or the response cannot be parsed; each case is logged.
public sealed class PlaceGeocoder
{
    private const Int32 SouthIndex = 0;
    private const Int32 NorthIndex = 1;
    private const Int32 WestIndex = 2;
    private const Int32 EastIndex = 3;
    private const Int32 BoundingBoxLength = 4;

    private readonly Func<String, Task<String>> _download;

    public PlaceGeocoder(Func<String, Task<String>> download) => this._download = download ?? throw new ArgumentNullException(nameof(download));

    public async Task<MapLocation?> FindAsync(String query)
    {
        if (String.IsNullOrWhiteSpace(query))
        {
            PluginLog.Warning("Place search skipped: empty query");
            return null;
        }

        var json = await this._download(String.Format(MapConstants.GeocodeUrlFormat, Uri.EscapeDataString(query.Trim())));
        if (json == null)
        {
            return null;
        }

        return TryParseFirstResult(json, out var location) ? location : LogNoMatch(query);
    }

    public static Boolean TryParseFirstResult(String json, out MapLocation location)
    {
        location = default;
        try
        {
            using var document = JsonDocument.Parse(json);
            return TryReadLocation(document.RootElement, out location);
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException or FormatException or KeyNotFoundException or ArgumentNullException)
        {
            PluginLog.Warning(ex, "Could not parse place search response");
            return false;
        }
    }

    private static Boolean TryReadLocation(JsonElement root, out MapLocation location)
    {
        location = default;
        if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
        {
            return false;
        }

        var place = root[0];
        var center = new GeoPoint(ReadCoordinate(place, "lat"), ReadCoordinate(place, "lon"));
        location = new MapLocation(center, ReadZoom(place));
        return true;
    }

    private static Double ReadCoordinate(JsonElement place, String propertyName) =>
        Double.Parse(place.GetProperty(propertyName).GetString(), CultureInfo.InvariantCulture);

    private static Int32 ReadZoom(JsonElement place)
    {
        if (!place.TryGetProperty("boundingbox", out var box) || box.GetArrayLength() != BoundingBoxLength)
        {
            return MapConstants.DefaultHome.Zoom;
        }

        var latitudeSpan = ReadBound(box, NorthIndex) - ReadBound(box, SouthIndex);
        var longitudeSpan = ReadBound(box, EastIndex) - ReadBound(box, WestIndex);
        return TileMath.ZoomToFit(Math.Max(latitudeSpan, longitudeSpan));
    }

    private static Double ReadBound(JsonElement box, Int32 index) =>
        Double.Parse(box[index].GetString(), CultureInfo.InvariantCulture);

    private static MapLocation? LogNoMatch(String query)
    {
        PluginLog.Warning($"No place found for '{query}'");
        return null;
    }
}
