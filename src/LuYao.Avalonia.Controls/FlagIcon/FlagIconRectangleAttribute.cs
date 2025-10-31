using System;

namespace LuYao.Avalonia.Controls;

/// <summary>
/// Specifies the rectangle coordinates for a flag icon in a sprite sheet
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class FlagIconRectangleAttribute : Attribute
{
    public FlagIconRectangleAttribute(int x, int y, int width, int height, bool isSmall = false)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        IsSmall = isSmall;
    }

    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }
    public bool IsSmall { get; }
}
