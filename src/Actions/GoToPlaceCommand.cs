namespace Loupedeck.MapsPlugin;

using Helpers;

public class GoToPlaceCommand : ActionEditorCommand
{
    private const String PlaceControlName = "place";

    public GoToPlaceCommand()
    {
        this.Name = "GoToPlace";
        this.DisplayName = "Go to Place";
        this.GroupName = MapConstants.GroupName;
        this.Description = "Searches OpenStreetMap for a place and makes it the home location of every map action";

        this.ActionEditor.AddControlEx(
            new ActionEditorTextbox(name: PlaceControlName, labelText: "Place:")
                .SetPlaceholder("e.g. Eiffel Tower, Paris")
                .SetRequired());
    }

    protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
    {
        if (!actionParameters.TryGetString(PlaceControlName, out var place) || String.IsNullOrWhiteSpace(place))
        {
            PluginLog.Warning("Go to Place pressed without a place name");
            return false;
        }

        // The search runs in the background so a slow Nominatim response does not block the device.
        _ = this.GoToPlaceAsync(place);
        return true;
    }

    private async Task GoToPlaceAsync(String place)
    {
        var location = await MapServices.Geocoder.FindAsync(place);
        if (location.HasValue)
        {
            ((MapsPlugin)this.Plugin).SaveHome(location.Value);
            PluginLog.Info($"Moved map to '{place}'");
        }
    }
}
