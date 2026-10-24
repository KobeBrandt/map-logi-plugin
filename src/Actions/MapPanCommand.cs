namespace Loupedeck.MapsPlugin;

using Helpers;

public class MapPanCommand : PluginDynamicCommand
{
    private const String PanGroupName = "Pan";
    private const String ParameterDisplayFormat = "Pan {0}";

    public MapPanCommand()
        : base()
    {
        this.DisplayName = "Pan Map";
        this.Description = "Moves the map half a view in one direction";
        this.GroupName = MapConstants.GroupName;
        foreach (var direction in Enum.GetValues<PanDirection>())
        {
            this.AddParameter(direction.ToString(), String.Format(ParameterDisplayFormat, direction), PanGroupName);
        }
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

    protected override void RunCommand(String actionParameter)
    {
        if (!PanDirections.TryParse(actionParameter, out var direction))
        {
            PluginLog.Warning($"Unknown pan direction '{actionParameter}'");
            return;
        }

        var (directionX, directionY) = PanDirections.ToVector(direction);
        MapServices.View.PanBy(directionX * MapConstants.PanStepPixels, directionY * MapConstants.PanStepPixels);
    }

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
    {
        using var builder = new BitmapBuilder(imageSize);
        MapServices.Renderer.Draw(builder, MapServices.View.Current);
        if (PanDirections.TryParse(actionParameter, out var direction))
        {
            SymbolRenderer.DrawArrow(builder, direction);
        }

        return builder.ToImage();
    }

    private void OnMapInvalidated(Object sender, EventArgs e) => this.ActionImageChanged();
}
