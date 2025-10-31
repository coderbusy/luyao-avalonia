using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace LuYao.Avalonia.Controls;

/// <summary>
/// A panel that arranges child elements with uniform spacing between them.
/// Supports both horizontal and vertical orientations.
/// </summary>
public class UniformSpacingPanel : Panel
{
    /// <summary>
    /// Defines the <see cref="Orientation"/> property.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<UniformSpacingPanel, Orientation>(
            nameof(Orientation),
            Orientation.Vertical);

    /// <summary>
    /// Defines the <see cref="Spacing"/> property.
    /// </summary>
    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<UniformSpacingPanel, double>(
            nameof(Spacing),
            0.0);

    /// <summary>
    /// Gets or sets the orientation in which child elements are arranged.
    /// The default value is Vertical.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the uniform spacing between child elements.
    /// The default value is 0.0.
    /// </summary>
    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    static UniformSpacingPanel()
    {
        AffectsMeasure<UniformSpacingPanel>(OrientationProperty, SpacingProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var isVertical = Orientation == Orientation.Vertical;
        var spacing = Spacing;
        var childCount = Children.Count;

        if (childCount == 0)
        {
            return new Size(0, 0);
        }

        var totalSpacing = spacing * (childCount - 1);
        
        Size childConstraint;
        double maxWidth = 0;
        double maxHeight = 0;

        if (isVertical)
        {
            // For vertical orientation, constrain width but allow height to grow
            childConstraint = new Size(availableSize.Width, double.PositiveInfinity);
            
            foreach (var child in Children)
            {
                child.Measure(childConstraint);
                var desiredSize = child.DesiredSize;
                maxWidth = Math.Max(maxWidth, desiredSize.Width);
                maxHeight += desiredSize.Height;
            }

            return new Size(maxWidth, maxHeight + totalSpacing);
        }
        else
        {
            // For horizontal orientation, constrain height but allow width to grow
            childConstraint = new Size(double.PositiveInfinity, availableSize.Height);
            
            foreach (var child in Children)
            {
                child.Measure(childConstraint);
                var desiredSize = child.DesiredSize;
                maxWidth += desiredSize.Width;
                maxHeight = Math.Max(maxHeight, desiredSize.Height);
            }

            return new Size(maxWidth + totalSpacing, maxHeight);
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var isVertical = Orientation == Orientation.Vertical;
        var spacing = Spacing;
        var position = 0.0;

        foreach (var child in Children)
        {
            var desiredSize = child.DesiredSize;
            
            if (isVertical)
            {
                // Arrange vertically
                var childRect = new Rect(
                    0,
                    position,
                    finalSize.Width,
                    desiredSize.Height);
                
                child.Arrange(childRect);
                position += desiredSize.Height + spacing;
            }
            else
            {
                // Arrange horizontally
                var childRect = new Rect(
                    position,
                    0,
                    desiredSize.Width,
                    finalSize.Height);
                
                child.Arrange(childRect);
                position += desiredSize.Width + spacing;
            }
        }

        return finalSize;
    }
}
