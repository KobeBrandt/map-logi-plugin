namespace Loupedeck.MapsPlugin.Helpers;

// The map location shared by every map action, so zooming on a dial moves the map on every button.
public sealed class MapView
{
    private readonly Object _lock = new();

    private MapLocation _current;
    private MapLocation _home;

    public MapView(MapLocation home)
    {
        this._home = home;
        this._current = home;
    }

    public event EventHandler Changed;

    public MapLocation Current
    {
        get
        {
            lock (this._lock)
            {
                return this._current;
            }
        }
    }

    public MapLocation Home
    {
        get
        {
            lock (this._lock)
            {
                return this._home;
            }
        }
    }

    public void ZoomBy(Int32 steps)
    {
        var current = this.Current;
        this.GoTo(current with { Zoom = TileMath.ClampZoom(current.Zoom + steps) });
    }

    public void PanBy(Double deltaX, Double deltaY)
    {
        var current = this.Current;
        var world = TileMath.ToWorld(current.Center, current.Zoom);
        var moved = TileMath.Move(world, deltaX, deltaY, current.Zoom);
        this.GoTo(current with { Center = TileMath.ToGeo(moved, current.Zoom) });
    }

    public void SetHome(MapLocation home)
    {
        lock (this._lock)
        {
            this._home = home;
        }

        this.GoTo(home);
    }

    public void ResetToHome() => this.GoTo(this.Home);

    public void GoTo(MapLocation location)
    {
        lock (this._lock)
        {
            if (this._current == location)
            {
                return;
            }

            this._current = location;
        }

        this.Changed?.Invoke(this, EventArgs.Empty);
    }
}
