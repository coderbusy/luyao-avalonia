# VirtualizingWrapPanel

A high-performance virtualizing wrap panel for Avalonia that supports both horizontal and vertical orientations with efficient memory usage.

## Features

- **Virtualization Support**: Only renders items in the visible viewport plus a buffer zone for optimal performance
- **Dual Orientation**: Supports both `Vertical` (default) and `Horizontal` orientations
- **Flexible Item Sizing**: Use `ItemSize` property or let the panel measure the first item
- **ItemsControl Integration**: Works seamlessly with Avalonia's ItemsControl
- **ScrollIntoView Support**: Allows programmatic scrolling to specific items
- **Stretch Support**: Optional item stretching to fill available space

## Installation

Add a reference to the `LuYao.Avalonia.Controls` project in your application.

## Usage

### Basic Example

```xaml
<ItemsControl ItemsSource="{Binding Items}">
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <ly:VirtualizingWrapPanel ItemSize="150, 100" 
                                      Orientation="Vertical" />
        </ItemsPanelTemplate>
    </ItemsControl.ItemsPanel>
    
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <Border Background="LightBlue" 
                    BorderBrush="Gray" 
                    BorderThickness="1">
                <TextBlock Text="{Binding Name}" 
                           HorizontalAlignment="Center"
                           VerticalAlignment="Center" />
            </Border>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

### Auto-sizing Items

If you don't specify `ItemSize`, the panel will measure the first item to determine the size:

```xaml
<ly:VirtualizingWrapPanel Orientation="Vertical" />
```
                    Background="{Binding IsHeader, Converter={StaticResource HeaderBackgroundConverter}}">
                <TextBlock Text="{Binding Name}" />
            </Border>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

### ViewModel Example

```csharp
public class MyViewModel
{
    public ObservableCollection<ItemViewModel> Items { get; }
    
    public MyViewModel()
    {
        Items = new ObservableCollection<ItemViewModel>();
        
        for (int i = 0; i < 1000; i++)
        {
            Items.Add(new ItemViewModel
            {
                Name = $"Item {i}"
            });
        }
    }
}

public class ItemViewModel
{
    public string Name { get; set; }
}
```

## Properties

### Orientation

- **Type**: `Orientation`
- **Default**: `Vertical`
- **Description**: Specifies the orientation in which items are arranged before wrapping.
  - `Vertical`: Items flow left-to-right, then wrap to next row (scrolls vertically)
  - `Horizontal`: Items flow top-to-bottom, then wrap to next column (scrolls horizontally)

### ItemSize

- **Type**: `Size`
- **Default**: `default(Size)` (0, 0)
- **Description**: Specifies the size of each item. If not set (or set to 0, 0), the panel will measure the first realized item to determine the size. This property is ignored if `ItemSizeProvider` is set.

**Example:**
```xaml
<ly:VirtualizingWrapPanel ItemSize="150, 100" />
```

### StretchItems

- **Type**: `bool`
- **Default**: `false`
- **Description**: When `true`, items are stretched to fill up remaining space in each row/column.

### ItemSizeProvider

- **Type**: `IItemSizeProvider?`
- **Default**: `null`
- **Description**: When set, provides custom sizes for individual items, enabling variable-sized items. Takes precedence over the `ItemSize` property.

**Example:**
```csharp
public class VariableItemSizeProvider : IItemSizeProvider
{
    public Size GetSizeForItem(int itemIndex)
    {
        // Return different sizes based on index
        return itemIndex % 2 == 0 
            ? new Size(100, 80)   // Even items
            : new Size(150, 120); // Odd items
    }
}
```

```xaml
<ly:VirtualizingWrapPanel ItemSizeProvider="{Binding MyItemSizeProvider}" />
```

## Layout Behavior

### Vertical Orientation (default)
- Items are arranged from left to right
- When a row is full, wrapping occurs to the next row below
- Scrolling occurs vertically
- When using `ItemSizeProvider`, row height adapts to the tallest item in each row

### Horizontal Orientation
- Items are arranged from top to bottom
- When a column is full, wrapping occurs to the next column to the right
- Scrolling occurs horizontally
- When using `ItemSizeProvider`, column width adapts to the widest item in each column

## Performance Considerations

- **Virtualization**: The panel only creates visual elements for items in the visible viewport plus a buffer zone
- **Element Recycling**: Implements an efficient recycling pool that reuses container elements when scrolling, significantly reducing GC pressure
- **Variable-Sized Items**: ItemSizeProvider is efficiently integrated into layout calculation with minimal performance impact
- **Large Datasets**: Designed to handle thousands of items efficiently through virtualization and recycling
- **Memory Usage**: Significantly reduces memory consumption compared to non-virtualizing panels
- **Scroll Performance**: Automatically updates displayed items during scrolling with minimal overhead

### Recycling Pool Benefits

The element recycling pool provides:
- **Reduced Allocations**: Containers are reused instead of created/destroyed on every scroll
- **Better Performance**: Less GC pressure leads to smoother scrolling
- **Memory Efficiency**: Maintains only the containers needed for visible items plus buffer
- **Automatic Management**: Recycling is handled transparently by the panel

## API Compatibility

This implementation follows the API design of the WPF VirtualizingWrapPanel for consistency:
- `ItemSize` instead of separate `ItemWidth`/`ItemHeight`
- `Orientation` property for direction control
- `ItemSizeProvider` interface for variable-sized items
- Standard wrap panel behavior without custom break line logic
- Element recycling pool matching Avalonia's VirtualizingStackPanel pattern

## Future Enhancements

- `SpacingMode` and `ItemAlignment` properties
- Grid layout mode option
- Enhanced ScrollIntoView with animation options
- Accessibility improvements

## Demo

Run the demo application included in the solution to see the VirtualizingWrapPanel in action. The demo includes:
- 10,000 sample items to demonstrate virtualization performance
- Toggle for variable item sizes (using ItemSizeProvider)
- Interactive demonstration of the layout behavior
- Item count display showing total items vs rendered items

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## License

This component is part of the LuYao.Avalonia project. See the main project LICENSE for details.
