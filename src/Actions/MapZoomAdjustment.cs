namespace Loupedeck.MapsPlugin;

using Helpers;

public class MapZoomAdjustment : PluginDynamicAdjustment
{
    private const String ZoomValueFormat = "Zoom {0}";

    public MapZoomAdjustment()
        : base("Map Zoom", "Turn to zoom the map in and out. Press to return to your home location.", MapConstants.GroupName, hasReset: true)
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

    protected override void ApplyAdjustment(String actionParameter, Int32 diff) => MapServices.View.ZoomBy(diff);

    protected override void RunCommand(String actionParameter) => MapServices.View.ResetToHome();

    protected override String GetAdjustmentValue(String actionParameter) =>
        String.Format(ZoomValueFormat, MapServices.View.Current.Zoom);

    protected override BitmapImage GetAdjustmentImage(String actionParameter, PluginImageSize imageSize) =>
        MapServices.Renderer.Render(MapServices.View.Current, imageSize);

    private void OnMapInvalidated(Object sender, EventArgs e)
    {
        this.ActionImageChanged();
        this.AdjustmentValueChanged();
    }
}
