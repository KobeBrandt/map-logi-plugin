namespace Loupedeck.MapsPlugin.Helpers;

public sealed class MapImageInvalidator
{
    public MapImageInvalidator(MapView view, TileProvider tiles)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(tiles);
        view.Changed += this.OnSourceChanged;
        tiles.TileLoaded += this.OnSourceChanged;
    }

    public event EventHandler Invalidated;

    private void OnSourceChanged(Object sender, EventArgs e) => this.Invalidated?.Invoke(this, EventArgs.Empty);
}
