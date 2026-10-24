namespace Loupedeck.MapsPlugin.Helpers;

public static class MapConstants
{
    public const String GroupName = "Maps";

    public const Int32 TileSize = 256;
    public const Int32 MinZoom = 0;
    public const Int32 MaxZoom = 19;
    public const Double MaxLatitude = 85.05112878;
    public const Double FullLongitudeSpan = 360.0;
    public const Double HalfLongitudeSpan = 180.0;

    public const Double PanStepPixels = TileSize / 2.0;

    public const String TileUrlFormat = "https://tile.openstreetmap.org/{0}/{1}/{2}.png";
    public const String GeocodeUrlFormat = "https://nominatim.openstreetmap.org/search?format=jsonv2&limit=1&q={0}";
    public const String BrowseUrlFormat = "https://www.openstreetmap.org/?mlat={1}&mlon={2}#map={0}/{1}/{2}";
    public const String UserAgent = "LogiMapsPlugin/1.0 (+https://github.com/KobeBrandt/map-logi-plugin)";
    public const String CoordinateFormat = "0.#####";

    public const String Attribution = "© OSM";
    public const Int32 AttributionFontSize = 9;
    public const Int32 AttributionHeight = 12;
    public const Int32 MarkerRadius = 4;
    public const Int32 MarkerBorder = 2;
    public const Single SymbolLineWidth = 4f;
    public const Single SymbolLengthRatio = 0.3f;
    public const Single ArrowHeadRatio = 0.12f;

    public const Int32 TileCacheCapacity = 512;
    public const Int32 MaxConcurrentDownloads = 2;

    public const String HomeLocationSetting = "HomeLocation";

    public const Int32 GridColumns = 3;
    public const Int32 GridRows = 3;
    public const Int32 BackButtonSlots = 1;
    public const Int32 ZoomStep = 1;

    public static readonly MapLocation DefaultHome = new(new GeoPoint(51.8885, -8.4084), 16);

    public static readonly BitmapColor BackgroundColor = new(170, 211, 223);
    public static readonly BitmapColor AttributionBackground = new(255, 255, 255, 170);
    public static readonly BitmapColor AttributionColor = new(40, 40, 40);
    public static readonly BitmapColor MarkerColor = new(214, 40, 40);
    public static readonly BitmapColor MarkerBorderColor = BitmapColor.White;
    public static readonly BitmapColor SymbolColor = BitmapColor.White;
    public static readonly BitmapColor SymbolShadeColor = new(0, 0, 0, 110);

    public static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(10);
    public static readonly TimeSpan FailedTileRetryDelay = TimeSpan.FromSeconds(30);
}
