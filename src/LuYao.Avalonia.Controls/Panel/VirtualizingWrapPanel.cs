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
/// A virtualizing wrap panel that supports both horizontal and vertical orientations.
/// Only renders items in the visible viewport plus a buffer zone for improved performance.
/// </summary>
public class VirtualizingWrapPanel : VirtualizingPanel
{
    /// <summary>
    /// Defines the <see cref="Orientation"/> property.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, Orientation>(
            nameof(Orientation),
            Orientation.Vertical);

    /// <summary>
    /// Defines the <see cref="ItemSize"/> property.
    /// </summary>
    public static readonly StyledProperty<Size> ItemSizeProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, Size>(
            nameof(ItemSize),
            default(Size));

    /// <summary>
    /// Defines the <see cref="StretchItems"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> StretchItemsProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, bool>(
            nameof(StretchItems),
            false);

    /// <summary>
    /// Gets or sets the orientation in which items are arranged before wrapping.
    /// The default value is Vertical.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of the items. The default value is Size.Empty.
    /// If the value is Size.Empty, the item size is determined by measuring the first realized item.
    /// </summary>
    public Size ItemSize
    {
        get => GetValue(ItemSizeProperty);
        set => SetValue(ItemSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether items get stretched to fill up remaining space.
    /// The default value is false.
    /// </summary>
    public bool StretchItems
    {
        get => GetValue(StretchItemsProperty);
        set => SetValue(StretchItemsProperty, value);
    }

    // Cache for calculated layout information
    private readonly List<LayoutInfo> _layoutCache = new();
    private readonly Dictionary<object, int> _itemIndexCache = new();
    private double _lastAvailableWidth = 0;
    private double _lastAvailableHeight = 0;
    private int _lastItemCount = 0;
    private readonly HashSet<int> _realizedIndexes = new();
    private ScrollViewer? _scrollViewer;
    private Size _measuredItemSize = default(Size);

    private class LayoutInfo
    {
        public int ItemIndex { get; set; }
        public Rect Bounds { get; set; }
    }

    static VirtualizingWrapPanel()
    {
        AffectsMeasure<VirtualizingWrapPanel>(OrientationProperty, ItemSizeProperty, StretchItemsProperty);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateScrollViewer();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        UnsubscribeFromScrollViewer();
    }

    private void UpdateScrollViewer()
    {
        var newScrollViewer = this.FindAncestorOfType<ScrollViewer>();
        if (newScrollViewer != _scrollViewer)
        {
            UnsubscribeFromScrollViewer();
            _scrollViewer = newScrollViewer;
            SubscribeToScrollViewer();
        }
    }

    private void SubscribeToScrollViewer()
    {
        if (_scrollViewer != null)
        {
            _scrollViewer.PropertyChanged += OnScrollViewerPropertyChanged;
        }
    }

    private void UnsubscribeFromScrollViewer()
    {
        if (_scrollViewer != null)
        {
            _scrollViewer.PropertyChanged -= OnScrollViewerPropertyChanged;
            _scrollViewer = null;
        }
    }

    private void OnScrollViewerPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == ScrollViewer.OffsetProperty)
        {
            // Invalidate measure when scroll offset changes
            InvalidateMeasure();
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var items = Items;
        var generator = ItemContainerGenerator;
        
        if (items == null || items.Count == 0 || generator == null)
        {
            return new Size(0, 0);
        }

        // Determine item size
        var itemSize = ItemSize;
        if (itemSize.Width == 0 || itemSize.Height == 0)
        {
            // Measure first item to determine size
            if (_measuredItemSize == default(Size) && items.Count > 0)
            {
                var firstContainer = GetOrCreateContainer(items[0], 0);
                if (firstContainer != null)
                {
                    firstContainer.Measure(availableSize);
                    _measuredItemSize = firstContainer.DesiredSize;
                }
            }
            itemSize = _measuredItemSize != default(Size) ? _measuredItemSize : new Size(100, 100);
        }

        var orientation = Orientation;
        var isVertical = orientation == Orientation.Vertical;
        var itemCount = items.Count;
        
        // Determine panel constraint
        var panelSize = isVertical 
            ? (double.IsInfinity(availableSize.Width) ? itemSize.Width * 4 : availableSize.Width)
            : (double.IsInfinity(availableSize.Height) ? itemSize.Height * 4 : availableSize.Height);
        
        // Calculate layout if needed
        var needsLayout = _layoutCache.Count == 0 
            || Math.Abs((isVertical ? _lastAvailableWidth : _lastAvailableHeight) - panelSize) > 0.01
            || _lastItemCount != itemCount;
            
        if (needsLayout)
        {
            CalculateLayout(panelSize, itemSize, itemCount, isVertical);
            if (isVertical)
                _lastAvailableWidth = panelSize;
            else
                _lastAvailableHeight = panelSize;
            _lastItemCount = itemCount;
            
            // Rebuild item index cache
            _itemIndexCache.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item != null)
                {
                    _itemIndexCache[item] = i;
                }
            }
        }

        // Get viewport info for virtualization
        var viewport = GetViewportInfo();
        var visibleRange = GetVisibleRange(viewport, isVertical);

        // Track which indexes should be realized
        var targetIndexes = new HashSet<int>();
        for (int i = visibleRange.start; i < visibleRange.start + visibleRange.count && i < itemCount; i++)
        {
            targetIndexes.Add(i);
        }

        // Remove containers that are no longer in the visible range
        var childrenToRemove = new List<Control>();
        foreach (var child in Children)
        {
            if (child is Control control)
            {
                var index = GetItemIndex(control.DataContext);
                if (index < 0 || !targetIndexes.Contains(index))
                {
                    childrenToRemove.Add(control);
                }
            }
        }

        foreach (var child in childrenToRemove)
        {
            RemoveInternalChild(child);
        }

        // Realize elements in the visible range
        _realizedIndexes.Clear();
        for (int i = visibleRange.start; i < visibleRange.start + visibleRange.count && i < itemCount; i++)
        {
            var item = items[i];
            var container = GetOrCreateContainer(item, i);
            
            if (container != null && i < _layoutCache.Count)
            {
                _realizedIndexes.Add(i);
                var layoutInfo = _layoutCache[i];
                var size = new Size(layoutInfo.Bounds.Width, layoutInfo.Bounds.Height);
                container.Measure(size);
            }
        }

        // Return total extent
        if (_layoutCache.Count > 0)
        {
            var lastItem = _layoutCache[_layoutCache.Count - 1];
            return isVertical 
                ? new Size(panelSize, lastItem.Bounds.Bottom)
                : new Size(lastItem.Bounds.Right, panelSize);
        }

        return isVertical 
            ? new Size(panelSize, 0)
            : new Size(0, panelSize);
    }

    private Control? GetOrCreateContainer(object? item, int index)
    {
        var generator = ItemContainerGenerator;
        if (generator == null)
            return null;

        // Check if container already exists
        foreach (var child in Children)
        {
            if (child is Control control && control.DataContext == item)
                return control;
        }

        // Create new container
        if (generator.NeedsContainer(item, index, out var recycleKey))
        {
            var container = generator.CreateContainer(item, index, recycleKey);
            generator.PrepareItemContainer(container, item, index);
            AddInternalChild(container);
            generator.ItemContainerPrepared(container, item, index);
            return container;
        }
        else
        {
            // Item is its own container
            if (item is Control itemControl)
            {
                AddInternalChild(itemControl);
                return itemControl;
            }
        }

        return null;
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

    private void CalculateLayout(double panelSize, Size itemSize, int itemCount, bool isVertical)
    {
        _layoutCache.Clear();

        if (isVertical)
        {
            // Vertical orientation: items flow left-to-right, then wrap to next row
            var itemsPerRow = Math.Max(1, (int)(panelSize / itemSize.Width));
            var currentX = 0.0;
            var currentY = 0.0;

            for (int i = 0; i < itemCount; i++)
            {
                if (currentX + itemSize.Width > panelSize && currentX > 0)
                {
                    // Move to next row
                    currentY += itemSize.Height;
                    currentX = 0;
                }

                _layoutCache.Add(new LayoutInfo
                {
                    ItemIndex = i,
                    Bounds = new Rect(currentX, currentY, itemSize.Width, itemSize.Height)
                });

                currentX += itemSize.Width;
            }
        }
        else
        {
            // Horizontal orientation: items flow top-to-bottom, then wrap to next column
            var itemsPerColumn = Math.Max(1, (int)(panelSize / itemSize.Height));
            var currentX = 0.0;
            var currentY = 0.0;

            for (int i = 0; i < itemCount; i++)
            {
                if (currentY + itemSize.Height > panelSize && currentY > 0)
                {
                    // Move to next column
                    currentX += itemSize.Width;
                    currentY = 0;
                }

                _layoutCache.Add(new LayoutInfo
                {
                    ItemIndex = i,
                    Bounds = new Rect(currentX, currentY, itemSize.Width, itemSize.Height)
                });

                currentY += itemSize.Height;
            }
        }
    }

    private (int start, int count) GetVisibleRange(in (double offset, double viewport) viewportInfo, bool isVertical)
    {
        if (_layoutCache.Count == 0)
            return (0, 0);

        var (offset, viewport) = viewportInfo;
        var start = -1;
        var end = -1;

        // Add buffer zone (one viewport above/below or left/right)
        var bufferZone = viewport;
        var visibleStart = Math.Max(0, offset - bufferZone);
        var visibleEnd = offset + viewport + bufferZone;

        // Find first and last visible items
        for (int i = 0; i < _layoutCache.Count; i++)
        {
            var layoutInfo = _layoutCache[i];
            var itemStart = isVertical ? layoutInfo.Bounds.Top : layoutInfo.Bounds.Left;
            var itemEnd = isVertical ? layoutInfo.Bounds.Bottom : layoutInfo.Bounds.Right;

            if (itemEnd >= visibleStart && itemStart <= visibleEnd)
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
            var isVertical = Orientation == Orientation.Vertical;
            return isVertical 
                ? (scrollViewer.Offset.Y, scrollViewer.Viewport.Height)
                : (scrollViewer.Offset.X, scrollViewer.Viewport.Width);
        }
        return (0, double.PositiveInfinity);
    }

    private int GetItemIndex(object? item)
    {
        if (item == null)
            return -1;

        // Try cache first
        if (_itemIndexCache.TryGetValue(item, out var cachedIndex))
            return cachedIndex;

        // Fallback to linear search if not in cache
        var items = Items;
        if (items == null)
            return -1;

        for (int i = 0; i < items.Count; i++)
        {
            if (Equals(items[i], item))
            {
                _itemIndexCache[item] = i;
                return i;
            }
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
