using System;

namespace LuYao.Avalonia.Controls;

/// <summary>
/// Specifies the rectangle coordinates for a flag icon in the regular sprite sheet
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class FlagIconRectangleAttribute : Attribute
{
    public FlagIconRectangleAttribute(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }
}
