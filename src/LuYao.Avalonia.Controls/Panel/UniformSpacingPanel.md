# UniformSpacingPanel

A panel control that arranges child elements with uniform spacing between them.

## Features

- **Orientation Support**: Supports both Vertical and Horizontal orientations
- **Configurable Spacing**: Adjustable uniform spacing between child elements
- **Simple and Efficient**: Lightweight panel implementation based on Avalonia's Panel class

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Orientation` | `Orientation` | `Vertical` | The orientation in which child elements are arranged |
| `Spacing` | `double` | `0.0` | The uniform spacing between child elements in pixels |

## Usage

### XAML

```xml
<luyao:UniformSpacingPanel Orientation="Vertical" Spacing="10">
    <Button Content="First Button" />
    <Button Content="Second Button" />
    <Button Content="Third Button" />
</luyao:UniformSpacingPanel>
```

### Vertical Orientation

When `Orientation="Vertical"`, children are arranged from top to bottom with uniform spacing between them:

```
┌─────────────┐
│   Child 1   │
├─────────────┤  ← Spacing
│   Child 2   │
├─────────────┤  ← Spacing
│   Child 3   │
└─────────────┘
```

### Horizontal Orientation

When `Orientation="Horizontal"`, children are arranged from left to right with uniform spacing between them:

```
┌────┬───┬────┬───┬────┐
│ C1 │ S │ C2 │ S │ C3 │
└────┴───┴────┴───┴────┘
       ↑       ↑
    Spacing  Spacing
```

## Demo

The demo application includes an interactive example where you can:
- Switch between Vertical and Horizontal orientations
- Adjust spacing from 0 to 50 pixels using a slider
- See real-time updates to the layout

Navigate to **Panels → UniformSpacingPanel** in the demo application to see it in action.

## Implementation Details

The `UniformSpacingPanel` extends Avalonia's `Panel` class and implements custom measure and arrange logic:

- **MeasureOverride**: Calculates the desired size based on children's sizes and spacing
- **ArrangeOverride**: Positions children with uniform spacing based on the orientation

## Tests

Comprehensive unit tests are included covering:
- Default property values
- Property setters
- Measure logic for both orientations
- Arrange logic for both orientations
- Spacing calculations
- Edge cases (no children, zero spacing)

All 13 tests pass successfully.
