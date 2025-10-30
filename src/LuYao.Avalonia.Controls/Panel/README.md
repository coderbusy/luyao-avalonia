# VirtualizingWrapPanel

A high-performance virtualizing wrap panel for Avalonia that supports vertical scrolling with an `IsBreakLine` attached property.

## Features

- **Virtualization Support**: Only renders items in the visible viewport plus a buffer zone for optimal performance
- **Fixed Item Size**: Requires `ItemWidth` and `ItemHeight` for consistent layout calculation
- **IsBreakLine Property**: Supports full-width items via the `IsBreakLine` attached property
- **Vertical Scrolling**: Optimized for vertical scroll scenarios with large data sets
- **ItemsControl Integration**: Works seamlessly with Avalonia's ItemsControl
- **ScrollIntoView Support**: Allows programmatic scrolling to specific items

## Installation

Add a reference to the `LuYao.Avalonia.Controls` project in your application.

## Usage

### Basic Example

```xaml
<ItemsControl ItemsSource="{Binding Items}">
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <ly:VirtualizingWrapPanel ItemWidth="150" 
                                      ItemHeight="100" />
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

### Using IsBreakLine

The `IsBreakLine` attached property allows certain items to occupy the full width of the panel:

```xaml
<ItemsControl ItemsSource="{Binding Items}">
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <ly:VirtualizingWrapPanel ItemWidth="150" 
                                      ItemHeight="100" />
        </ItemsPanelTemplate>
    </ItemsControl.ItemsPanel>
    
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <Border ly:VirtualizingWrapPanel.IsBreakLine="{Binding IsHeader}"
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
                Name = $"Item {i}",
                IsHeader = (i % 10 == 0) // Every 10th item is a header
            });
        }
    }
}

public class ItemViewModel
{
    public string Name { get; set; }
    public bool IsHeader { get; set; }
}
```

## Properties

### ItemWidth

- **Type**: `double`
- **Default**: 100
- **Description**: The width of each item in the panel. Required for layout calculation.

### ItemHeight

- **Type**: `double`
- **Default**: 100
- **Description**: The height of each item in the panel. Required for layout calculation.

## Attached Properties

### IsBreakLine

- **Type**: `bool`
- **Default**: `false`
- **Description**: When set to `true`, the item will occupy the full width of the panel and force a new row.

**Usage:**
```xaml
<Border ly:VirtualizingWrapPanel.IsBreakLine="True">
    <!-- Content -->
</Border>
```

Or data-bound:
```xaml
<Border ly:VirtualizingWrapPanel.IsBreakLine="{Binding IsBreakLine}">
    <!-- Content -->
</Border>
```

## Layout Behavior

1. **Normal Items**: Items are arranged from left to right, wrapping to the next row when the panel width is exceeded
2. **Break Line Items**: When an item has `IsBreakLine="True"`:
   - If not at the start of a row, a new row is started
   - The item occupies the full width of the panel
   - The next item starts on a new row

## Performance Considerations

- **Virtualization**: The panel only creates visual elements for items in the visible viewport plus a buffer zone
- **Fixed Size**: Items must have fixed sizes (ItemWidth and ItemHeight) for optimal performance
- **Large Datasets**: Designed to handle thousands of items efficiently through virtualization
- **Memory Usage**: Significantly reduces memory consumption compared to non-virtualizing panels

## Current Limitations

- **Horizontal Scrolling**: Currently optimized for vertical scrolling only
- **Variable Item Sizes**: Items must have uniform sizes (specified by ItemWidth and ItemHeight)
- **Full Virtualization**: Element recycling is still being optimized

## Future Enhancements

- Complete element recycling implementation for maximum performance
- Support for variable item sizes
- Horizontal orientation support
- Enhanced ScrollIntoView with animation options
- Accessibility improvements

## Demo

Run the demo application included in the solution to see the VirtualizingWrapPanel in action. The demo includes:
- 100 sample items
- Break line items every 10th item
- Interactive demonstration of the layout behavior

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## License

This component is part of the LuYao.Avalonia project. See the main project LICENSE for details.
