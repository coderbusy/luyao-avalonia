using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuYao.Avalonia.Controls;

/// <summary>
/// A virtualizing wrap panel that supports vertical scrolling and the IsBreakLine attached property.
/// Only renders items in the visible viewport plus a buffer zone for improved performance.
/// </summary>
public class VirtualizingWrapPanel : VirtualizingPanel
{
    private const double DefaultItemWidth = 100;
    private const double DefaultItemHeight = 100;
    
    /// <summary>
    /// Defines the <see cref="ItemWidth"/> property.
    /// </summary>
    public static readonly StyledProperty<double> ItemWidthProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, double>(
            nameof(ItemWidth), 
            DefaultItemWidth,
            coerce: CoerceItemSize);

    /// <summary>
    /// Defines the <see cref="ItemHeight"/> property.
    /// </summary>
    public static readonly StyledProperty<double> ItemHeightProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, double>(
            nameof(ItemHeight), 
            DefaultItemHeight,
            coerce: CoerceItemSize);

    /// <summary>
    /// Defines the IsBreakLine attached property.
    /// When true, the item will occupy the full width of the panel.
    /// </summary>
    public static readonly AttachedProperty<bool> IsBreakLineProperty =
        AvaloniaProperty.RegisterAttached<VirtualizingWrapPanel, Control, bool>("IsBreakLine");

    /// <summary>
    /// Gets or sets the width of each item in the panel.
    /// </summary>
    public double ItemWidth
    {
        get => GetValue(ItemWidthProperty);
        set => SetValue(ItemWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of each item in the panel.
    /// </summary>
    public double ItemHeight
    {
        get => GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }

    /// <summary>
    /// Gets the IsBreakLine attached property value for a control.
    /// </summary>
    public static bool GetIsBreakLine(Control control)
    {
        return control.GetValue(IsBreakLineProperty);
    }

    /// <summary>
    /// Sets the IsBreakLine attached property value for a control.
    /// </summary>
    public static void SetIsBreakLine(Control control, bool value)
    {
        control.SetValue(IsBreakLineProperty, value);
    }

    private static double CoerceItemSize(AvaloniaObject obj, double value)
    {
        return Math.Max(1, value);
    }

    // Cache for calculated layout information
    private readonly List<LayoutInfo> _layoutCache = new();
    private double _lastAvailableWidth = 0;
    private int _lastItemCount = 0;

    private class LayoutInfo
    {
        public int ItemIndex { get; set; }
        public Rect Bounds { get; set; }
        public bool IsBreakLine { get; set; }
    }

    static VirtualizingWrapPanel()
    {
        AffectsMeasure<VirtualizingWrapPanel>(ItemWidthProperty, ItemHeightProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var items = Items;
        if (items == null || items.Count == 0)
        {
            return new Size(0, 0);
        }

        var itemWidth = ItemWidth;
        var itemHeight = ItemHeight;
        var panelWidth = double.IsInfinity(availableSize.Width) ? itemWidth * 4 : availableSize.Width;
        
        // Calculate layout if needed
        var itemCount = items.Count;
        if (_layoutCache.Count == 0 || 
            Math.Abs(_lastAvailableWidth - panelWidth) > 0.01 || 
            _lastItemCount != itemCount)
        {
            CalculateLayout(panelWidth, itemWidth, itemHeight, itemCount);
            _lastAvailableWidth = panelWidth;
            _lastItemCount = itemCount;
        }

        // Get visible range
        var viewport = GetViewportInfo();
        var visibleRange = GetVisibleRange(viewport);

        // Realize visible items
        var generator = ItemContainerGenerator;
        if (generator != null && visibleRange.count > 0)
        {
            // Measure items in the visible range
            for (int i = visibleRange.start; i < visibleRange.start + visibleRange.count && i < itemCount; i++)
            {
                var container = generator.ContainerFromIndex(i);
                if (container != null && container is Control control)
                {
                    if (i < _layoutCache.Count)
                    {
                        var layoutInfo = _layoutCache[i];
                        var size = new Size(layoutInfo.Bounds.Width, layoutInfo.Bounds.Height);
                        control.Measure(size);
                    }
                }
            }
        }

        // Return total extent
        if (_layoutCache.Count > 0)
        {
            var lastItem = _layoutCache[_layoutCache.Count - 1];
            var totalHeight = lastItem.Bounds.Bottom;
            return new Size(panelWidth, totalHeight);
        }

        return new Size(panelWidth, 0);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var children = Children;
        
        foreach (var child in children)
        {
            if (child is Control control)
            {
                var dataContext = control.DataContext;
                var index = GetItemIndex(dataContext);
                
                if (index >= 0 && index < _layoutCache.Count)
                {
                    var layoutInfo = _layoutCache[index];
                    child.Arrange(layoutInfo.Bounds);
                }
            }
        }

        return finalSize;
    }

    private void CalculateLayout(double panelWidth, double itemWidth, double itemHeight, int itemCount)
    {
        _layoutCache.Clear();

        var itemsPerRow = Math.Max(1, (int)(panelWidth / itemWidth));
        var currentX = 0.0;
        var currentY = 0.0;
        var currentRowHeight = itemHeight;

        for (int i = 0; i < itemCount; i++)
        {
            var isBreakLine = IsItemBreakLine(i);

            if (isBreakLine)
            {
                // Break line item occupies full width
                if (currentX > 0)
                {
                    // Move to next row if not at start of row
                    currentY += currentRowHeight;
                    currentX = 0;
                    currentRowHeight = itemHeight;
                }

                _layoutCache.Add(new LayoutInfo
                {
                    ItemIndex = i,
                    Bounds = new Rect(0, currentY, panelWidth, itemHeight),
                    IsBreakLine = true
                });

                currentY += itemHeight;
                currentX = 0;
                currentRowHeight = itemHeight;
            }
            else
            {
                // Normal item
                if (currentX + itemWidth > panelWidth && currentX > 0)
                {
                    // Move to next row
                    currentY += currentRowHeight;
                    currentX = 0;
                    currentRowHeight = itemHeight;
                }

                _layoutCache.Add(new LayoutInfo
                {
                    ItemIndex = i,
                    Bounds = new Rect(currentX, currentY, itemWidth, itemHeight),
                    IsBreakLine = false
                });

                currentX += itemWidth;
            }
        }
    }

    private bool IsItemBreakLine(int index)
    {
        var items = Items;
        if (items == null) return false;

        var item = items.ElementAtOrDefault(index);
        if (item == null) return false;

        // Check if we have a realized control for this item
        foreach (var child in Children)
        {
            if (child is Control control && control.DataContext == item)
            {
                return GetIsBreakLine(control);
            }
        }

        return false;
    }

    private (int start, int count) GetVisibleRange(in (double offset, double viewport) viewportInfo)
    {
        if (_layoutCache.Count == 0)
            return (0, 0);

        var (offset, viewport) = viewportInfo;
        var start = -1;
        var end = -1;

        // Add buffer zone (one viewport above and below)
        var bufferZone = viewport;
        var visibleStart = Math.Max(0, offset - bufferZone);
        var visibleEnd = offset + viewport + bufferZone;

        // Find first and last visible items
        for (int i = 0; i < _layoutCache.Count; i++)
        {
            var layoutInfo = _layoutCache[i];
            var itemTop = layoutInfo.Bounds.Top;
            var itemBottom = layoutInfo.Bounds.Bottom;

            if (itemBottom >= visibleStart && itemTop <= visibleEnd)
            {
                if (start == -1)
                    start = i;
                end = i;
            }
        }

        if (start == -1)
            return (0, 0);

        return (start, end - start + 1);
    }

    private (double offset, double viewport) GetViewportInfo()
    {
        var scrollViewer = this.FindAncestorOfType<ScrollViewer>();
        if (scrollViewer != null)
        {
            return (scrollViewer.Offset.Y, scrollViewer.Viewport.Height);
        }
        return (0, double.PositiveInfinity);
    }

    private int GetItemIndex(object? item)
    {
        var items = Items;
        if (items == null || item == null)
            return -1;

        for (int i = 0; i < items.Count; i++)
        {
            if (Equals(items[i], item))
                return i;
        }
        return -1;
    }

    // VirtualizingPanel abstract methods implementation
    
    protected override Control? ContainerFromIndex(int index)
    {
        foreach (var child in Children)
        {
            if (child is Control control)
            {
                var itemIndex = GetItemIndex(control.DataContext);
                if (itemIndex == index)
                    return control;
            }
        }
        return null;
    }

    protected override int IndexFromContainer(Control container)
    {
        return GetItemIndex(container.DataContext);
    }

    protected override IEnumerable<Control>? GetRealizedContainers()
    {
        return Children.OfType<Control>();
    }

    protected override Control? ScrollIntoView(int index)
    {
        if (index < 0 || index >= _layoutCache.Count)
            return null;

        var layoutInfo = _layoutCache[index];
        var scrollViewer = this.FindAncestorOfType<ScrollViewer>();
        
        if (scrollViewer != null)
        {
            var targetY = layoutInfo.Bounds.Top;
            scrollViewer.Offset = new Vector(scrollViewer.Offset.X, targetY);
        }

        return ContainerFromIndex(index);
    }

    protected override IInputElement? GetControl(NavigationDirection direction, IInputElement? from, bool wrap)
    {
        // Simple implementation - can be enhanced based on requirements
        var fromControl = from as Control;
        if (fromControl == null)
            return Children.FirstOrDefault() as Control;

        var fromIndex = IndexFromContainer(fromControl);
        if (fromIndex == -1)
            return null;

        var targetIndex = fromIndex;
        var items = Items;
        var itemCount = items?.Count ?? 0;
        
        switch (direction)
        {
            case NavigationDirection.Next:
            case NavigationDirection.Down:
            case NavigationDirection.Right:
                targetIndex = fromIndex + 1;
                break;
            case NavigationDirection.Previous:
            case NavigationDirection.Up:
            case NavigationDirection.Left:
                targetIndex = fromIndex - 1;
                break;
            case NavigationDirection.First:
                targetIndex = 0;
                break;
            case NavigationDirection.Last:
                targetIndex = itemCount - 1;
                break;
        }

        if (wrap)
        {
            if (targetIndex < 0)
                targetIndex = itemCount - 1;
            else if (targetIndex >= itemCount)
                targetIndex = 0;
        }
        else
        {
            if (targetIndex < 0 || targetIndex >= itemCount)
                return null;
        }

        return ContainerFromIndex(targetIndex);
    }
}
