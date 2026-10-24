namespace Loupedeck.MapsPlugin;

public class MapsApplication : ClientApplication
{
    protected override String GetProcessName() => String.Empty;

    protected override String GetBundleName() => String.Empty;

    public override ClientApplicationStatus GetApplicationStatus() => ClientApplicationStatus.Unknown;
}
