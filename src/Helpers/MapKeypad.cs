namespace Loupedeck.MapsPlugin.Helpers;

// What each map folder key does. The center key switches between panning and zoom mode.
public sealed class MapKeypad
{
    private readonly MapView _view;
    private volatile Boolean _zoomMode;

    public MapKeypad(MapView view) => this._view = view ?? throw new ArgumentNullException(nameof(view));

    public Boolean IsZoomMode => this._zoomMode;

    // Returns true when the mode changed, so the caller knows every key image must be redrawn.
    public Boolean Press(GridCell cell)
    {
        if (MapGrid.IsCenter(cell))
        {
            this._zoomMode = !this._zoomMode;
            return true;
        }

        if (this.TryGetActiveZoomStep(cell, out var zoomStep))
        {
            this._view.ZoomBy(zoomStep);
        }
        else
        {
            this._view.GoTo(MapGrid.LocationOf(this._view.Current, cell));
        }

        return false;
    }

    public Boolean TryGetActiveZoomStep(GridCell cell, out Int32 zoomStep)
    {
        zoomStep = 0;
        return this._zoomMode && MapGrid.TryGetZoomStep(cell, out zoomStep);
    }

    public void LeaveZoomMode() => this._zoomMode = false;
}
