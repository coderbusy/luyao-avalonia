# FlagIcon

FlagIcon provides flag icons for countries and regions as an attached property that can be applied to Avalonia Image controls.

## Features

- **Individual PNG files**: All 270+ flag images are stored as individual PNG files for maximum compatibility
- **External scaling**: Scaling is handled by the Image control - set Width and Height as needed
- **Easy to use**: Simple attached property syntax in XAML with string-based codes
- **AOT-compatible**: No reflection used, fully compatible with Native AOT compilation
- **Fallback support**: Unknown flag codes automatically display the "XX" (unknown) flag

## Usage

### Basic Usage

```xaml
<Image ly:FlagIcon.Code="CN" Width="100" />
<Image ly:FlagIcon.Code="US" Width="100" />
```

### Controlling Size

Control the size using the Image control's Width and Height properties:

```xaml
<!-- Large flags -->
<Image ly:FlagIcon.Code="CN" Width="100" Height="75" />

<!-- Small/thumbnail flags -->
<Image ly:FlagIcon.Code="US" Width="20" Height="15" />

<!-- Custom size - aspect ratio maintained by Image control -->
<Image ly:FlagIcon.Code="GB" Width="50" />
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
- Each flag PNG is 100x75 pixels at base resolution
- Scaling is handled by the Image control itself
- String-based codes provide flexibility and type safety
- AOT-compatible without reflection

## Migration from Sprite-based FlagIcon

If you were using the old sprite-based FlagIcon:

The API has been simplified - the `UseSmall` property has been removed. Control sizing through the Image control's Width and Height properties:

```xaml
<!-- Before (with UseSmall) -->
<Image ly:FlagIcon.Code="CN" ly:FlagIcon.UseSmall="True" Width="20" />

<!-- After (without UseSmall) -->
<Image ly:FlagIcon.Code="CN" Width="20" Height="15" />
```
