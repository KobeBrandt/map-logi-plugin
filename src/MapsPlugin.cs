namespace Loupedeck.MapsPlugin;

using Helpers;

public class MapsPlugin : Plugin
{
    public override Boolean UsesApplicationApiOnly => true;

    public override Boolean HasNoApplication => true;

    public MapsPlugin()
    {
        PluginLog.Init(this.Log);
        PluginResources.Init(this.Assembly);
    }

    public override void Load()
    {
        if (this.TryGetPluginSetting(MapConstants.HomeLocationSetting, out var saved)
            && MapLocationSerializer.TryDeserialize(saved, out var home))
        {
            MapServices.View.SetHome(home);
        }
    }

    public override void Unload()
    {
    }

    public void SaveHome(MapLocation home)
    {
        MapServices.View.SetHome(home);
        this.SetPluginSetting(MapConstants.HomeLocationSetting, MapLocationSerializer.Serialize(home));
    }
}
