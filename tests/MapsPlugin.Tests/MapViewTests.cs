namespace Loupedeck.MapsPlugin.Tests;

using Helpers;

using Xunit;

public class MapViewTests
{
    private static readonly MapLocation Home = new(new GeoPoint(50.8466, 4.3528), 13);

    [Fact]
    public void ZoomBy_ClampsToSupportedZoomRange()
    {
        var view = new MapView(Home);

        view.ZoomBy(100);
        Assert.Equal(MapConstants.MaxZoom, view.Current.Zoom);

        view.ZoomBy(-100);
        Assert.Equal(MapConstants.MinZoom, view.Current.Zoom);
    }

    [Fact]
    public void ZoomBy_AtLimit_DoesNotRaiseChanged()
    {
        var view = new MapView(Home with { Zoom = MapConstants.MaxZoom });
        var raised = 0;
        view.Changed += (_, _) => raised++;

        view.ZoomBy(1);

        Assert.Equal(0, raised);
    }

    [Fact]
    public void PanBy_East_IncreasesLongitude()
    {
        var view = new MapView(Home);

        view.PanBy(MapConstants.PanStepPixels, 0);

        Assert.True(view.Current.Center.Longitude > Home.Center.Longitude);
        Assert.Equal(Home.Center.Latitude, view.Current.Center.Latitude, 1e-9);
    }

    [Fact]
    public void PanBy_North_IncreasesLatitude()
    {
        var view = new MapView(Home);

        view.PanBy(0, -MapConstants.PanStepPixels);

        Assert.True(view.Current.Center.Latitude > Home.Center.Latitude);
    }

    [Fact]
    public void ResetToHome_ReturnsToHomeAfterMoving()
    {
        var view = new MapView(Home);
        view.PanBy(MapConstants.PanStepPixels, MapConstants.PanStepPixels);
        view.ZoomBy(2);

        view.ResetToHome();

        Assert.Equal(Home, view.Current);
    }

    [Fact]
    public void SetHome_MovesTheViewAndRaisesChanged()
    {
        var view = new MapView(Home);
        var paris = new MapLocation(new GeoPoint(48.8584, 2.2945), 16);
        var raised = 0;
        view.Changed += (_, _) => raised++;

        view.SetHome(paris);

        Assert.Equal(paris, view.Current);
        Assert.Equal(paris, view.Home);
        Assert.Equal(1, raised);
    }
}
