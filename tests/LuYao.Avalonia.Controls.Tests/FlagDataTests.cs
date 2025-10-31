using Avalonia;
using Xunit;

namespace LuYao.Avalonia.Controls.Tests;

public class FlagDataTests
{
    [Fact]
    public void GetRegularFlags_ShouldReturn272Flags()
    {
        // Act
        var flags = FlagData.GetRegularFlags();
        var count = 0;
        foreach (var _ in flags)
        {
            count++;
        }

        // Assert
        Assert.Equal(272, count);
    }

    [Fact]
    public void GetSmallFlags_ShouldReturn272Flags()
    {
        // Act
        var flags = FlagData.GetSmallFlags();
        var count = 0;
        foreach (var _ in flags)
        {
            count++;
        }

        // Assert
        Assert.Equal(272, count);
    }

    [Fact]
    public void GetRegularFlags_ShouldHaveCorrectCoordinates()
    {
        // Arrange
        var flags = new List<(string Code, PixelRect Rect)>(FlagData.GetRegularFlags());

        // Assert - Check first flag (AC)
        Assert.Equal("AC", flags[0].Code);
        Assert.Equal(new PixelRect(0, 0, 100, 75), flags[0].Rect);

        // Check a middle flag (CN should be at position 48)
        var cnFlag = flags.FirstOrDefault(f => f.Code == "CN");
        Assert.Equal("CN", cnFlag.Code);
        Assert.Equal(new PixelRect(1600, 150, 100, 75), cnFlag.Rect);

        // Check last flag (ZW)
        Assert.Equal("ZW", flags[271].Code);
        Assert.Equal(new PixelRect(1600, 1125, 100, 75), flags[271].Rect);
    }

    [Fact]
    public void GetRegularFlags_ShouldIncludeSpecialCodes()
    {
        // Arrange
        var flags = FlagData.GetRegularFlags().Select(f => f.Code).ToList();

        // Assert - Check that special codes are included
        Assert.Contains("AC", flags);  // Ascension Island
        Assert.Contains("ARAB", flags); // Arab League
        Assert.Contains("CEFTA", flags); // CEFTA
        Assert.Contains("EA", flags); // Ceuta & Melilla
        Assert.Contains("EAC", flags); // East African Community
        Assert.Contains("TA", flags); // Tristan da Cunha
        Assert.Contains("XX", flags); // Unknown/fallback
    }

    [Fact]
    public void GetRegularFlags_ShouldHaveUniqueCodesAndPositions()
    {
        // Arrange
        var flags = FlagData.GetRegularFlags().ToList();

        // Assert - Check all codes are unique
        var codes = flags.Select(f => f.Code).ToList();
        Assert.Equal(codes.Count, codes.Distinct().Count());

        // Check all positions are unique
        var positions = flags.Select(f => (f.Rect.X, f.Rect.Y)).ToList();
        Assert.Equal(positions.Count, positions.Distinct().Count());
    }

    [Fact]
    public void GetRegularFlags_ShouldFollowGridPattern()
    {
        // Arrange
        var flags = FlagData.GetRegularFlags().ToList();

        // Assert - Check that positions follow a 17-column grid pattern
        for (int i = 0; i < flags.Count; i++)
        {
            var expectedCol = i % 17;
            var expectedRow = i / 17;
            var expectedX = expectedCol * 100;
            var expectedY = expectedRow * 75;

            Assert.Equal(expectedX, flags[i].Rect.X);
            Assert.Equal(expectedY, flags[i].Rect.Y);
            Assert.Equal(100, flags[i].Rect.Width);
            Assert.Equal(75, flags[i].Rect.Height);
        }
    }

    [Fact]
    public void GetSmallFlags_ShouldFollowGridPattern()
    {
        // Arrange
        var flags = FlagData.GetSmallFlags().ToList();

        // Assert - Check that positions follow a 17-column grid pattern
        for (int i = 0; i < flags.Count; i++)
        {
            var expectedCol = i % 17;
            var expectedRow = i / 17;
            var expectedX = expectedCol * 20;
            var expectedY = expectedRow * 15;

            Assert.Equal(expectedX, flags[i].Rect.X);
            Assert.Equal(expectedY, flags[i].Rect.Y);
            Assert.Equal(20, flags[i].Rect.Width);
            Assert.Equal(15, flags[i].Rect.Height);
        }
    }
}
