using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using LuYao.Avalonia.Controls;
using Xunit;

namespace LuYao.Avalonia.Controls.Tests;

public class LightTextBlockTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var control = new LightTextBlock();

        // Assert
        Assert.Equal(string.Empty, control.Text);
        Assert.Null(control.FontFamily);
        Assert.Equal(12.0, control.FontSize);
        Assert.Equal(Brushes.Black, control.Foreground);
    }

    [Fact]
    public void Text_ShouldBeSettable()
    {
        // Arrange
        var control = new LightTextBlock();
        var text = "Hello, World!";

        // Act
        control.Text = text;

        // Assert
        Assert.Equal(text, control.Text);
    }

    [Fact]
    public void FontSize_ShouldBeSettable()
    {
        // Arrange
        var control = new LightTextBlock();
        var fontSize = 16.0;

        // Act
        control.FontSize = fontSize;

        // Assert
        Assert.Equal(fontSize, control.FontSize);
    }

    [Fact]
    public void Foreground_ShouldBeSettable()
    {
        // Arrange
        var control = new LightTextBlock();
        var foreground = Brushes.Red;

        // Act
        control.Foreground = foreground;

        // Assert
        Assert.Equal(foreground, control.Foreground);
    }

    [Fact]
    public void FontFamily_ShouldBeSettable()
    {
        // Arrange
        var control = new LightTextBlock();
        var fontFamily = new FontFamily("Arial");

        // Act
        control.FontFamily = fontFamily;

        // Assert
        Assert.Equal(fontFamily, control.FontFamily);
    }

    [Theory]
    [InlineData(12.0)]
    [InlineData(16.0)]
    [InlineData(20.0)]
    public void FontSize_ShouldAcceptVariousValues(double fontSize)
    {
        // Arrange
        var control = new LightTextBlock();

        // Act
        control.FontSize = fontSize;

        // Assert
        Assert.Equal(fontSize, control.FontSize);
    }

    [Fact]
    public void Control_ShouldInheritFromControl()
    {
        // Arrange & Act
        var control = new LightTextBlock();

        // Assert
        Assert.IsAssignableFrom<Control>(control);
    }

    [Fact]
    public void TextProperty_ShouldBeStyledProperty()
    {
        // Arrange & Act
        var property = LightTextBlock.TextProperty;

        // Assert
        Assert.NotNull(property);
        Assert.Equal(nameof(LightTextBlock.Text), property.Name);
    }

    [Fact]
    public void FontSizeProperty_ShouldBeStyledProperty()
    {
        // Arrange & Act
        var property = LightTextBlock.FontSizeProperty;

        // Assert
        Assert.NotNull(property);
        Assert.Equal(nameof(LightTextBlock.FontSize), property.Name);
    }

    [Fact]
    public void ForegroundProperty_ShouldBeStyledProperty()
    {
        // Arrange & Act
        var property = LightTextBlock.ForegroundProperty;

        // Assert
        Assert.NotNull(property);
        Assert.Equal(nameof(LightTextBlock.Foreground), property.Name);
    }

    [Fact]
    public void FontFamilyProperty_ShouldBeStyledProperty()
    {
        // Arrange & Act
        var property = LightTextBlock.FontFamilyProperty;

        // Assert
        Assert.NotNull(property);
        Assert.Equal(nameof(LightTextBlock.FontFamily), property.Name);
    }
}
