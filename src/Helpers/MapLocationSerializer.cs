namespace Loupedeck.MapsPlugin.Helpers;

using System.Globalization;

// Stores a location as "latitude;longitude;zoom" in invariant culture so plugin settings survive locale changes.
public static class MapLocationSerializer
{
    private const Char Separator = ';';
    private const Int32 PartCount = 3;

    public static String Serialize(MapLocation location) => String.Join(
        Separator,
        location.Center.Latitude.ToString(CultureInfo.InvariantCulture),
        location.Center.Longitude.ToString(CultureInfo.InvariantCulture),
        location.Zoom.ToString(CultureInfo.InvariantCulture));

    public static Boolean TryDeserialize(String text, out MapLocation location)
    {
        location = default;
        var parts = text?.Split(Separator);
        if (parts?.Length != PartCount
            || !TryParseDouble(parts[0], out var latitude)
            || !TryParseDouble(parts[1], out var longitude)
            || !Int32.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var zoom))
        {
            return false;
        }

        location = new MapLocation(new GeoPoint(TileMath.ClampLatitude(latitude), longitude), TileMath.ClampZoom(zoom));
        return true;
    }

    private static Boolean TryParseDouble(String text, out Double value) =>
        Double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
}
