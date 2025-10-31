# FlagIcon

FlagIcon provides flag icons for countries and regions as an attached property that can be applied to Avalonia Image controls.

## Features

- **Individual PNG files**: All 270+ flag images are stored as individual PNG files for maximum compatibility
- **Dynamic scaling**: Images are automatically scaled based on display DPI and requested size
- **Two modes**: Regular (100x75) and Small (thumbnail) modes available
- **Easy to use**: Simple attached property syntax in XAML with string-based codes
- **AOT-compatible**: No reflection used, fully compatible with Native AOT compilation
- **Fallback support**: Unknown flag codes automatically display the "XX" (unknown) flag

## Usage

### Basic Usage

```xaml
<Image ly:FlagIcon.Code="CN" Width="100" />
<Image ly:FlagIcon.Code="US" Width="100" />
```

### Using Small Flags

For smaller icons/thumbnails, use the `UseSmall` property:

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

- Flag icons are loaded from individual PNG files on demand
- Images are automatically scaled based on DPI and UseSmall property
- Each flag PNG is 100x75 pixels at base resolution
- When UseSmall is true, images are decoded to 20x15 effective size
- String-based codes provide flexibility and type safety
- AOT-compatible without reflection

## Migration from Sprite-based FlagIcon

If you were using the old sprite-based FlagIcon:

The API remains the same - no code changes needed! The implementation now uses individual PNG files instead of sprite sheets for better compatibility and easier maintenance.

```xaml
<!-- Same usage as before -->
<Image ly:FlagIcon.Code="CN" Width="100" />
<Image ly:FlagIcon.Code="US" ly:FlagIcon.UseSmall="True" Width="20" />
```
