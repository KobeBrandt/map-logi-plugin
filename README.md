# Maps for OpenStreetMap

Browse OpenStreetMap from a Logitech MX Keypad. Every map action shares one map view, so turning the zoom dial
updates the map on every button at once.

This plugin was made to test how far the Logitech MX Creative Keypad could be taken: can nine small LCD keys work
together as one live, interactive map?

Map data © [OpenStreetMap contributors](https://www.openstreetmap.org/copyright). Unofficial plugin, not affiliated with
or endorsed by Logitech or the OpenStreetMap Foundation.

## Actions

| Action | Type | What it does |
| --- | --- | --- |
| **Map** | Dynamic command | Shows the live map. Press to open the same view on openstreetmap.org. |
| **Map Zoom** | Dynamic adjustment | Turn to zoom in and out. Press to return to the home location. |
| **Pan Map** (North, East, South, West) | Dynamic command | Moves the map half a view. Each button shows the map with an arrow. |
| **Go to Place** | Action editor command | Searches a place name with Nominatim and makes it the home location. |
| **Map Folder** | Dynamic folder | Fills a 3x3 page (MX Creative Keypad) with one large map. The top-left key stays Back, because the keypad always puts it there. Press an outer button to center the map on it; press the center button to switch to zoom mode, where the keys above and below the center zoom in and out; press the center again to leave zoom mode. On devices with encoders, turn to zoom and press to return home. |

The home location is saved in the plugin settings. It defaults to the Logitech office in Cork, Ireland.

## How it works

- Tiles come from `tile.openstreetmap.org` and are drawn with the Web Mercator math in `TileMath`.
- Button images are drawn synchronously, so missing tiles download in the background. The buttons redraw once a tile
  arrives.
- Tiles are kept in a 512-tile in-memory cache. At most two downloads run at a time, and a failed tile waits 30 seconds
  before a retry, as the [tile usage policy](https://operations.osmfoundation.org/policies/tiles/) asks.
- Every request sends an identifying `User-Agent`, which OpenStreetMap requires.

## Requirements

- [Logi Options+](https://www.logitech.com/software/logi-options-plus.html) with the Logi Plugin Service
- [.NET SDK](https://dotnet.microsoft.com/download) to build from source
- An internet connection for map tiles and place search

## Build and test

```bash
dotnet build MapsPlugin.sln
```

```bash
dotnet test MapsPlugin.sln
```

The build links `bin/Debug` into the Logi Plugin Service and asks the service to reload the plugin.

## Package and install

```bash
dotnet build -c Release
```

```bash
logiplugintool pack ./bin/Release ./Maps.lplug4
```

```bash
logiplugintool install ./Maps.lplug4
```

## License

[MIT](LICENSE). Map tiles and place data are © OpenStreetMap contributors and are licensed under the
[ODbL](https://www.openstreetmap.org/copyright), not under this repository's license.
