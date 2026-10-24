namespace Loupedeck.MapsPlugin.Tests;

using Helpers;

using Xunit;

public class MapKeypadTests
{
    private static readonly MapLocation Home = new(new GeoPoint(50.8466, 4.3528), 13);
    private static readonly GridCell Center = new(1, 1);
    private static readonly GridCell Top = new(1, 0);
    private static readonly GridCell Bottom = new(1, 2);

    private readonly MapView _view = new(Home);

    [Fact]
    public void Constructor_NullView_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new MapKeypad(null));

    [Fact]
    public void Press_TopKeyOutsideZoomMode_PansNorth()
    {
        Assert.False(new MapKeypad(this._view).Press(Top));

        Assert.Equal(Home.Zoom, this._view.Current.Zoom);
        Assert.True(this._view.Current.Center.Latitude > Home.Center.Latitude);
    }

    [Fact]
    public void Press_Center_EntersZoomModeAndAsksForRedraw()
    {
        var keypad = new MapKeypad(this._view);

        Assert.True(keypad.Press(Center));
        Assert.True(keypad.IsZoomMode);
        Assert.Equal(Home, this._view.Current);
    }

    [Fact]
    public void Press_ZoomMode_TopZoomsInAndBottomZoomsOut()
    {
        var keypad = new MapKeypad(this._view);
        keypad.Press(Center);

        keypad.Press(Top);
        Assert.Equal(Home with { Zoom = Home.Zoom + MapConstants.ZoomStep }, this._view.Current);

        keypad.Press(Bottom);
        Assert.Equal(Home, this._view.Current);
    }

    [Fact]
    public void Press_CenterTwice_LeavesZoomMode()
    {
        var keypad = new MapKeypad(this._view);
        keypad.Press(Center);
        keypad.Press(Center);

        Assert.False(keypad.IsZoomMode);
        Assert.False(keypad.TryGetActiveZoomStep(Top, out _));
    }

    [Fact]
    public void LeaveZoomMode_ResetsToPanning()
    {
        var keypad = new MapKeypad(this._view);
        keypad.Press(Center);

        keypad.LeaveZoomMode();

        Assert.False(keypad.IsZoomMode);
    }
}
