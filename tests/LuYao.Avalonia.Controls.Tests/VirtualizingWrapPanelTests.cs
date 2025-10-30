using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using LuYao.Avalonia.Controls;
using Xunit;

namespace LuYao.Avalonia.Controls.Tests;

public class VirtualizingWrapPanelTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var panel = new VirtualizingWrapPanel();

        // Assert
        Assert.Equal(Orientation.Vertical, panel.Orientation);
        Assert.Equal(default(Size), panel.ItemSize);
        Assert.False(panel.StretchItems);
    }

    [Fact]
    public void Orientation_ShouldBeSettable()
    {
        // Arrange
        var panel = new VirtualizingWrapPanel();

        // Act
        panel.Orientation = Orientation.Horizontal;

        // Assert
        Assert.Equal(Orientation.Horizontal, panel.Orientation);
    }

    [Fact]
    public void ItemSize_ShouldBeSettable()
    {
        // Arrange
        var panel = new VirtualizingWrapPanel();
        var size = new Size(150, 100);

        // Act
        panel.ItemSize = size;

        // Assert
        Assert.Equal(size, panel.ItemSize);
    }

    [Fact]
    public void StretchItems_ShouldBeSettable()
    {
        // Arrange
        var panel = new VirtualizingWrapPanel();

        // Act
        panel.StretchItems = true;

        // Assert
        Assert.True(panel.StretchItems);
    }

    [Theory]
    [InlineData(Orientation.Vertical)]
    [InlineData(Orientation.Horizontal)]
    public void Orientation_ShouldAcceptBothValues(Orientation orientation)
    {
        // Arrange
        var panel = new VirtualizingWrapPanel();

        // Act
        panel.Orientation = orientation;

        // Assert
        Assert.Equal(orientation, panel.Orientation);
    }

    [Fact]
    public void Panel_ShouldInheritFromVirtualizingPanel()
    {
        // Arrange & Act
        var panel = new VirtualizingWrapPanel();

        // Assert
        Assert.IsAssignableFrom<VirtualizingPanel>(panel);
    }
}
