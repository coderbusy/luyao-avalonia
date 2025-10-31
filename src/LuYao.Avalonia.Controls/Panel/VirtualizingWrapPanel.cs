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
    /// Defines the <see cref="ItemSizeProvider"/> property.
    /// </summary>
    public static readonly StyledProperty<IItemSizeProvider?> ItemSizeProviderProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, IItemSizeProvider?>(
            nameof(ItemSizeProvider),
            null);

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
    /// This property is ignored if ItemSizeProvider is set.
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

    /// <summary>
    /// Gets or sets the item size provider for variable-sized items.
    /// When set, this takes precedence over the ItemSize property.
    /// </summary>
    public IItemSizeProvider? ItemSizeProvider
    {
        get => GetValue(ItemSizeProviderProperty);
        set => SetValue(ItemSizeProviderProperty, value);
    }

    // Cache for calculated layout information
    private readonly List<LayoutInfo> _layoutCache = new();
    private readonly Dictionary<object, int> _itemIndexCache = new();
    private double _lastAvailableWidth = 0;
    private double _lastAvailableHeight = 0;
    private int _lastItemCount = 0;
    private readonly HashSet<int> _realizedIndexes = new();
    private Size _measuredItemSize = default(Size);
    private Rect _viewport;
    private Rect _extendedViewport;
    private bool _isWaitingForViewportUpdate;
    
    // Element recycling pool
    private Dictionary<object, Stack<Control>>? _recyclePool;
    
    private static readonly AttachedProperty<object?> RecycleKeyProperty =
        AvaloniaProperty.RegisterAttached<VirtualizingWrapPanel, Control, object?>("RecycleKey");
    
    private static readonly object s_itemIsItsOwnContainer = new object();

    private class LayoutInfo
    {
        public int ItemIndex { get; set; }
        public Rect Bounds { get; set; }
    }

    static VirtualizingWrapPanel()
    {
        AffectsMeasure<VirtualizingWrapPanel>(OrientationProperty, ItemSizeProperty, StretchItemsProperty);
    }

    public VirtualizingWrapPanel()
    {
        EffectiveViewportChanged += OnEffectiveViewportChanged;
    }

    private void OnEffectiveViewportChanged(object? sender, EffectiveViewportChangedEventArgs e)
    {
        var isVertical = Orientation == Orientation.Vertical;
        
        // Update current viewport
        _viewport = e.EffectiveViewport.Intersect(new Rect(Bounds.Size));
        _isWaitingForViewportUpdate = false;

        // Calculate extended viewport with buffer
        var viewportSize = isVertical ? _viewport.Height : _viewport.Width;
        var bufferSize = viewportSize; // 1x viewport size buffer on each side

        if (isVertical)
        {
            _extendedViewport = new Rect(
                _viewport.X,
                Math.Max(0, _viewport.Y - bufferSize),
                _viewport.Width,
                _viewport.Height + (2 * bufferSize)
            );
        }
        else
        {
            _extendedViewport = new Rect(
                Math.Max(0, _viewport.X - bufferSize),
                _viewport.Y,
                _viewport.Width + (2 * bufferSize),
                _viewport.Height
            );
        }

        // Trigger remeasure with new viewport
        InvalidateMeasure();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var items = Items;
        var generator = ItemContainerGenerator;
        
        if (items == null || items.Count == 0 || generator == null)
        {
            return new Size(0, 0);
        }

        // If we're bringing an item into view, ignore any layout passes until we receive a new viewport
        if (_isWaitingForViewportUpdate)
        {
            return DesiredSize;
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

        // Use extended viewport for virtualization - includes buffer zone
        var viewport = _extendedViewport;
        var visibleRange = GetVisibleRange(viewport, isVertical);

        // Track which indexes should be realized
        var targetIndexes = new HashSet<int>();
        for (int i = visibleRange.start; i < visibleRange.start + visibleRange.count && i < itemCount; i++)
        {
            targetIndexes.Add(i);
        }

        // Recycle containers that are no longer in the visible range
        var childrenToRecycle = new List<(Control control, int index)>();
        foreach (var child in Children)
        {
            if (child is Control control)
            {
                var index = GetItemIndex(control.DataContext);
                if (index < 0 || !targetIndexes.Contains(index))
                {
                    childrenToRecycle.Add((control, index));
                }
            }
        }

        foreach (var (control, index) in childrenToRecycle)
        {
            RecycleElement(control, index);
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

        // Check if container already exists in children
        foreach (var child in Children)
        {
            if (child is Control control && control.DataContext == item)
                return control;
        }

        // Try to get or create container
        if (generator.NeedsContainer(item, index, out var recycleKey))
        {
            // Try to get from recycle pool first
            var recycled = GetRecycledElement(item, index, recycleKey);
            if (recycled != null)
                return recycled;
            
            // Create new container
            return CreateElement(item, index, recycleKey);
        }
        else
        {
            // Item is its own container
            return GetItemAsOwnContainer(item, index);
        }
    }
    
    private Control? GetRecycledElement(object? item, int index, object? recycleKey)
    {
        if (recycleKey == null || ItemContainerGenerator == null)
            return null;

        var generator = ItemContainerGenerator;

        if (_recyclePool?.TryGetValue(recycleKey, out var recyclePool) == true && recyclePool.Count > 0)
        {
            var recycled = recyclePool.Pop();
            recycled.SetCurrentValue(Visual.IsVisibleProperty, true);
            generator.PrepareItemContainer(recycled, item, index);
            generator.ItemContainerPrepared(recycled, item, index);
            return recycled;
        }

        return null;
    }
    
    private Control CreateElement(object? item, int index, object? recycleKey)
    {
        if (ItemContainerGenerator == null)
            throw new InvalidOperationException("ItemContainerGenerator is null");

        var generator = ItemContainerGenerator;
        var container = generator.CreateContainer(item, index, recycleKey);

        container.SetValue(RecycleKeyProperty, recycleKey);
        generator.PrepareItemContainer(container, item, index);
        AddInternalChild(container);
        generator.ItemContainerPrepared(container, item, index);

        return container;
    }
    
    private Control GetItemAsOwnContainer(object? item, int index)
    {
        if (item is not Control itemControl)
            throw new InvalidOperationException("Item must be a Control when it's its own container");

        itemControl.SetValue(RecycleKeyProperty, s_itemIsItsOwnContainer);
        
        if (ItemContainerGenerator != null)
        {
            ItemContainerGenerator.PrepareItemContainer(itemControl, item, index);
            ItemContainerGenerator.ItemContainerPrepared(itemControl, item, index);
        }
        
        AddInternalChild(itemControl);
        return itemControl;
    }
    
    private void RecycleElement(Control element, int index)
    {
        if (ItemContainerGenerator == null)
            return;

        var recycleKey = element.GetValue(RecycleKeyProperty);

        if (recycleKey == null)
        {
            // No recycle key, just remove it
            RemoveInternalChild(element);
        }
        else if (recycleKey == s_itemIsItsOwnContainer)
        {
            // Item is its own container, just hide it
            element.SetCurrentValue(Visual.IsVisibleProperty, false);
        }
        else
        {
            // Recycle the element
            ItemContainerGenerator.ClearItemContainer(element);
            PushToRecyclePool(recycleKey, element);
            element.SetCurrentValue(Visual.IsVisibleProperty, false);
        }
    }
    
    private void PushToRecyclePool(object recycleKey, Control element)
    {
        _recyclePool ??= new Dictionary<object, Stack<Control>>();

        if (!_recyclePool.TryGetValue(recycleKey, out var pool))
        {
            pool = new Stack<Control>();
            _recyclePool.Add(recycleKey, pool);
        }

        pool.Push(element);
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
        var sizeProvider = ItemSizeProvider;

        if (isVertical)
        {
            // Vertical orientation: items flow left-to-right, then wrap to next row
            var currentX = 0.0;
            var currentY = 0.0;
            var currentRowHeight = 0.0;

            for (int i = 0; i < itemCount; i++)
            {
                // Get item size from provider or use default
                var currentItemSize = sizeProvider?.GetSizeForItem(i) ?? itemSize;
                
                if (currentX + currentItemSize.Width > panelSize && currentX > 0)
                {
                    // Move to next row
                    currentY += currentRowHeight;
                    currentX = 0;
                    currentRowHeight = 0;
                }

                _layoutCache.Add(new LayoutInfo
                {
                    ItemIndex = i,
                    Bounds = new Rect(currentX, currentY, currentItemSize.Width, currentItemSize.Height)
                });

                currentX += currentItemSize.Width;
                currentRowHeight = Math.Max(currentRowHeight, currentItemSize.Height);
            }
        }
        else
        {
            // Horizontal orientation: items flow top-to-bottom, then wrap to next column
            var currentX = 0.0;
            var currentY = 0.0;
            var currentColumnWidth = 0.0;

            for (int i = 0; i < itemCount; i++)
            {
                // Get item size from provider or use default
                var currentItemSize = sizeProvider?.GetSizeForItem(i) ?? itemSize;
                
                if (currentY + currentItemSize.Height > panelSize && currentY > 0)
                {
                    // Move to next column
                    currentX += currentColumnWidth;
                    currentY = 0;
                    currentColumnWidth = 0;
                }

                _layoutCache.Add(new LayoutInfo
                {
                    ItemIndex = i,
                    Bounds = new Rect(currentX, currentY, currentItemSize.Width, currentItemSize.Height)
                });

                currentY += currentItemSize.Height;
                currentColumnWidth = Math.Max(currentColumnWidth, currentItemSize.Width);
            }
        }
    }

    private (int start, int count) GetVisibleRange(Rect viewport, bool isVertical)
    {
        if (_layoutCache.Count == 0)
            return (0, 0);

        var start = -1;
        var end = -1;

        var visibleStart = isVertical ? viewport.Top : viewport.Left;
        var visibleEnd = isVertical ? viewport.Bottom : viewport.Right;

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
