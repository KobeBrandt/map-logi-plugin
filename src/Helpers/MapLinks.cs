namespace Loupedeck.MapsPlugin.Helpers;

using System.Diagnostics;
using System.Globalization;

public static class MapLinks
{
    public static String ForLocation(MapLocation location) => String.Format(
        CultureInfo.InvariantCulture,
        MapConstants.BrowseUrlFormat,
        location.Zoom,
        location.Center.Latitude.ToString(MapConstants.CoordinateFormat, CultureInfo.InvariantCulture),
        location.Center.Longitude.ToString(MapConstants.CoordinateFormat, CultureInfo.InvariantCulture));

    // UseShellExecute hands the URL to the default browser. Fails when no browser is registered; that is logged.
    public static Boolean OpenInBrowser(String url)
    {
        try
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            return true;
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            PluginLog.Error(ex, $"Could not open {url}");
            return false;
        }
    }
}
