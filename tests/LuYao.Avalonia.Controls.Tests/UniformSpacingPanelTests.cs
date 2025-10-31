using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using LuYao.Avalonia.Controls;
using Xunit;

namespace LuYao.Avalonia.Controls.Tests;

public class UniformSpacingPanelTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var panel = new UniformSpacingPanel();

        // Assert
        Assert.Equal(Orientation.Vertical, panel.Orientation);
        Assert.Equal(0.0, panel.Spacing);
    }

    [Fact]
    public void Orientation_ShouldBeSettable()
    {
        // Arrange
        var panel = new UniformSpacingPanel();

        // Act
        panel.Orientation = Orientation.Horizontal;

        // Assert
        Assert.Equal(Orientation.Horizontal, panel.Orientation);
    }

    [Fact]
    public void Spacing_ShouldBeSettable()
    {
        // Arrange
        var panel = new UniformSpacingPanel();

        // Act
        panel.Spacing = 10.0;

        // Assert
        Assert.Equal(10.0, panel.Spacing);
    }

    [Theory]
    [InlineData(Orientation.Vertical)]
    [InlineData(Orientation.Horizontal)]
    public void Orientation_ShouldAcceptBothValues(Orientation orientation)
    {
        // Arrange
        var panel = new UniformSpacingPanel();

        // Act
        panel.Orientation = orientation;

        // Assert
        Assert.Equal(orientation, panel.Orientation);
    }

    [Fact]
    public void Panel_ShouldInheritFromPanel()
    {
        // Arrange & Act
        var panel = new UniformSpacingPanel();

        // Assert
        Assert.IsAssignableFrom<Panel>(panel);
    }

    [Fact]
    public void MeasureOverride_WithNoChildren_ShouldReturnZeroSize()
    {
        // Arrange
        var panel = new UniformSpacingPanel();
        var availableSize = new Size(100, 100);

        // Act
        panel.Measure(availableSize);

        // Assert
        Assert.Equal(new Size(0, 0), panel.DesiredSize);
    }

    [Fact]
    public void MeasureOverride_VerticalOrientation_ShouldCalculateCorrectSize()
    {
        // Arrange
        var panel = new UniformSpacingPanel
        {
            Orientation = Orientation.Vertical,
            Spacing = 10.0
        };

        // Add test children
        var child1 = new Border { Width = 50, Height = 30 };
        var child2 = new Border { Width = 60, Height = 40 };
        panel.Children.Add(child1);
        panel.Children.Add(child2);

        // Act
        panel.Measure(new Size(100, 100));

        // Assert
        // Width should be max of children (60)
        // Height should be sum of heights + spacing (30 + 40 + 10)
        Assert.Equal(60, panel.DesiredSize.Width);
        Assert.Equal(80, panel.DesiredSize.Height);
    }

    [Fact]
    public void MeasureOverride_HorizontalOrientation_ShouldCalculateCorrectSize()
    {
        // Arrange
        var panel = new UniformSpacingPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 10.0
        };

        // Add test children
        var child1 = new Border { Width = 50, Height = 30 };
        var child2 = new Border { Width = 60, Height = 40 };
        panel.Children.Add(child1);
        panel.Children.Add(child2);

        // Act
        panel.Measure(new Size(200, 100));

        // Assert
        // Width should be sum of widths + spacing (50 + 60 + 10)
        // Height should be max of children (40)
        Assert.Equal(120, panel.DesiredSize.Width);
        Assert.Equal(40, panel.DesiredSize.Height);
    }

    [Fact]
    public void MeasureOverride_WithThreeChildren_ShouldCalculateCorrectSpacing()
    {
        // Arrange
        var panel = new UniformSpacingPanel
        {
            Orientation = Orientation.Vertical,
            Spacing = 5.0
        };

        // Add three children
        panel.Children.Add(new Border { Width = 50, Height = 20 });
        panel.Children.Add(new Border { Width = 50, Height = 20 });
        panel.Children.Add(new Border { Width = 50, Height = 20 });

        // Act
        panel.Measure(new Size(100, 100));

        // Assert
        // Height should be sum of heights + 2 spacings (20 + 20 + 20 + 5 + 5)
        Assert.Equal(70, panel.DesiredSize.Height);
    }

    [Fact]
    public void MeasureOverride_WithZeroSpacing_ShouldNotAddExtraSpace()
    {
        // Arrange
        var panel = new UniformSpacingPanel
        {
            Orientation = Orientation.Vertical,
            Spacing = 0.0
        };

        panel.Children.Add(new Border { Width = 50, Height = 20 });
        panel.Children.Add(new Border { Width = 50, Height = 30 });

        // Act
        panel.Measure(new Size(100, 100));

        // Assert
        // Height should be sum of heights with no spacing (20 + 30)
        Assert.Equal(50, panel.DesiredSize.Height);
    }

    [Fact]
    public void ArrangeOverride_VerticalOrientation_ShouldPositionChildrenCorrectly()
    {
        // Arrange
        var panel = new UniformSpacingPanel
        {
            Orientation = Orientation.Vertical,
            Spacing = 10.0
        };

        var child1 = new Border { Width = 50, Height = 30 };
        var child2 = new Border { Width = 60, Height = 40 };
        panel.Children.Add(child1);
        panel.Children.Add(child2);

        // Act
        panel.Measure(new Size(100, 200));
        panel.Arrange(new Rect(0, 0, 100, 200));

        // Assert
        // Child1 should be at Y=0
        Assert.Equal(0, child1.Bounds.Y);
        Assert.Equal(30, child1.Bounds.Height);
        
        // Child2 should be at Y=30+10=40
        Assert.Equal(40, child2.Bounds.Y);
        Assert.Equal(40, child2.Bounds.Height);
    }

    [Fact]
    public void ArrangeOverride_HorizontalOrientation_ShouldPositionChildrenCorrectly()
    {
        // Arrange
        var panel = new UniformSpacingPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 10.0
        };

        var child1 = new Border { Width = 50, Height = 30 };
        var child2 = new Border { Width = 60, Height = 40 };
        panel.Children.Add(child1);
        panel.Children.Add(child2);

        // Act
        panel.Measure(new Size(200, 100));
        panel.Arrange(new Rect(0, 0, 200, 100));

        // Assert
        // Child1 should be at X=0
        Assert.Equal(0, child1.Bounds.X);
        Assert.Equal(50, child1.Bounds.Width);
        
        // Child2 should be at X=50+10=60
        Assert.Equal(60, child2.Bounds.X);
        Assert.Equal(60, child2.Bounds.Width);
    }
}
