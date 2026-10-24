namespace Loupedeck.MapsPlugin;

using Helpers;

public class MapDynamicFolder : PluginDynamicFolder
{
    private const String ZoomParameter = "Zoom";
    private const String ZoomValueFormat = "Zoom {0}";

    private readonly MapKeypad _keypad = new(MapServices.View);

    public MapDynamicFolder()
    {
        this.DisplayName = "Map Folder";
        this.Description = "Fills a 3x3 keypad with one large map next to the Back key. Press a button to center the map there; press the center to switch to zoom mode.";
        this.GroupName = MapConstants.GroupName;
    }

    // The keypad always shows Back in the top-left slot, so MapGrid leaves that slot out of the map.
    public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType _) => PluginDynamicFolderNavigation.ButtonArea;

    public override Boolean Activate()
    {
        MapServices.Invalidator.Invalidated += this.OnMapInvalidated;
        return base.Activate();
    }

    public override Boolean Deactivate()
    {
        MapServices.Invalidator.Invalidated -= this.OnMapInvalidated;
        this._keypad.LeaveZoomMode();
        return base.Deactivate();
    }

    public override IEnumerable<String> GetButtonPressActionNames(DeviceType _) =>
        MapGrid.Cells().Select(cell => this.CreateCommandName(MapGrid.ToParameter(cell)));

    public override IEnumerable<String> GetEncoderRotateActionNames(DeviceType _) => [this.CreateAdjustmentName(ZoomParameter)];

    public override IEnumerable<String> GetEncoderPressActionNames(DeviceType _) => [this.CreateCommandName(ZoomParameter)];

    public override BitmapImage GetButtonImage(PluginImageSize imageSize) =>
        MapServices.Renderer.Render(MapServices.View.Current, imageSize);

    // An empty name stops the service labelling each slice with its action parameter.
    public override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => String.Empty;

    public override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
    {
        if (!MapGrid.TryParse(actionParameter, out var cell))
        {
            return null;
        }

        using var builder = new BitmapBuilder(imageSize);
        MapServices.Renderer.Draw(builder, MapGrid.LocationOf(MapServices.View.Current, cell), MapGrid.OverlaysFor(cell));
        if (this._keypad.TryGetActiveZoomStep(cell, out var zoomStep))
        {
            SymbolRenderer.DrawZoomSign(builder, zoomStep);
        }

        return builder.ToImage();
    }

    public override void RunCommand(String actionParameter)
    {
        if (actionParameter == ZoomParameter)
        {
            MapServices.View.ResetToHome();
        }
        else if (!MapGrid.TryParse(actionParameter, out var cell))
        {
            PluginLog.Warning($"Unknown map folder button '{actionParameter}'");
        }
        else if (this._keypad.Press(cell))
        {
            this.RefreshCellImages();
        }
    }

    public override void ApplyAdjustment(String actionParameter, Int32 diff) => MapServices.View.ZoomBy(diff);

    public override String GetAdjustmentValue(String actionParameter) =>
        String.Format(ZoomValueFormat, MapServices.View.Current.Zoom);

    private void OnMapInvalidated(Object sender, EventArgs e)
    {
        this.RefreshCellImages();
        this.AdjustmentValueChanged(ZoomParameter);
    }

    private void RefreshCellImages()
    {
        foreach (var cell in MapGrid.Cells())
        {
            this.CommandImageChanged(MapGrid.ToParameter(cell));
        }
    }
}
