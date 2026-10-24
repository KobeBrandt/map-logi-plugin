namespace Loupedeck.MapsPlugin.Helpers;

[Flags]
public enum MapOverlays
{
    None = 0,
    CenterMarker = 1,
    Attribution = 2,
    All = CenterMarker | Attribution,
}
