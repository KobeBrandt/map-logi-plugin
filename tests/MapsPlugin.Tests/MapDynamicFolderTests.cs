namespace Loupedeck.MapsPlugin.Tests;

using Helpers;

using Xunit;

public class MapDynamicFolderTests
{
    [Fact]
    public void Navigation_PutsBackInTheButtonAreaThatMapGridSkips() =>
        Assert.Equal(PluginDynamicFolderNavigation.ButtonArea, new MapDynamicFolder().GetNavigationArea(DeviceType.None));

    [Fact]
    public void GetCommandDisplayName_IsEmptySoSlicesHaveNoLabel() =>
        Assert.Empty(new MapDynamicFolder().GetCommandDisplayName("1", PluginImageSize.Width90));

    [Fact]
    public void GetCommandImage_UnknownParameter_ReturnsNull() =>
        Assert.Null(new MapDynamicFolder().GetCommandImage("not-a-cell", PluginImageSize.Width90));

    [Fact]
    public void RunCommand_UnknownParameter_LeavesTheMapUnchanged()
    {
        var before = MapServices.View.Current;

        new MapDynamicFolder().RunCommand("not-a-cell");

        Assert.Equal(before, MapServices.View.Current);
    }
}
