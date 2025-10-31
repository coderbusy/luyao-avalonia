# FlagIcon

FlagIcon provides flag icons for countries and regions as an attached property that can be applied to Avalonia Image controls.

## Features

- **Sprite-based**: All 270+ flag images are stored in optimized sprite sheets for efficient memory usage
- **Two sizes**: Regular (100x75) and Small (20x15) sprite sheets available
- **Easy to use**: Simple attached property syntax in XAML with string-based codes
- **Lightweight**: Only two sprite sheet images are distributed instead of 270+ individual files
- **AOT-compatible**: No reflection used, fully compatible with Native AOT compilation
- **Fallback support**: Unknown flag codes automatically display the "XX" (unknown) flag

## Usage

### Basic Usage

```xaml
<Image ly:FlagIcon.Code="CN" Width="100" />
<Image ly:FlagIcon.Code="US" Width="100" />
```

### Using Small Flags

For smaller icons, use the `UseSmall` property:

```xaml
<Image ly:FlagIcon.Code="CN" ly:FlagIcon.UseSmall="True" Width="20" />
<Image ly:FlagIcon.Code="US" ly:FlagIcon.UseSmall="True" Width="20" />
```

### Handling Unknown Codes

If an invalid or unknown code is provided, the "XX" flag will be displayed:

```xaml
<Image ly:FlagIcon.Code="INVALID" Width="100" />  <!-- Shows XX flag -->
<Image ly:FlagIcon.Code="" Width="100" />  <!-- Shows XX flag -->
```

### Available Flag Codes

FlagIcon supports string codes for:
- Countries (e.g., "CN", "US", "GB", "JP", etc.)
- Regions (e.g., "ES-CT" for Catalonia, "GB-SCT" for Scotland)
- Organizations (e.g., "EU" for European Union, "UN" for United Nations)

Codes are case-insensitive.

## Implementation Details

- Flag icons are loaded from sprite sheets at startup
- Images are cached in memory for efficient reuse
- Uses Avalonia's `CroppedBitmap` to extract individual flags from the sprite sheet
- Hardcoded position data ensures AOT compatibility without reflection
- String-based codes provide flexibility and type safety

## Migration from Old FlagIcon Control

If you were using the old `FlagIcon` control:

**Before:**
```xaml
<ly:FlagIcon Code="CN" />
```

**After:**
```xaml
<Image ly:FlagIcon.Code="CN" Width="22" />
```

Note: You now need to explicitly set the Width (and optionally Height) of the Image control.

## Migration from Enum-based Code Property

If you were using the enum-based Code property:

**Before:**
```xaml
<Image ly:FlagIcon.Code="CN" Width="22" />  <!-- FlagCode enum -->
```

**After:**
```xaml
<Image ly:FlagIcon.Code="CN" Width="22" />  <!-- String -->
```

The syntax remains the same, but now accepts string values instead of enum values, providing better flexibility and AOT compatibility.
