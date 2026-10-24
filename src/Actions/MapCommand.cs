namespace Loupedeck.MapsPlugin;

using Helpers;

public class MapCommand : PluginDynamicCommand
{
    public MapCommand()
        : base("Map", "Shows the current OpenStreetMap view. Press to open it in your browser.", MapConstants.GroupName)
    {
    }

    protected override Boolean OnLoad()
    {
        MapServices.Invalidator.Invalidated += this.OnMapInvalidated;
        return base.OnLoad();
    }

    protected override Boolean OnUnload()
    {
        MapServices.Invalidator.Invalidated -= this.OnMapInvalidated;
        return base.OnUnload();
    }

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
        MapServices.Renderer.Render(MapServices.View.Current, imageSize);

    protected override void RunCommand(String actionParameter) =>
        MapLinks.OpenInBrowser(MapLinks.ForLocation(MapServices.View.Current));

    private void OnMapInvalidated(Object sender, EventArgs e) => this.ActionImageChanged();
}
