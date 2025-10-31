# FlagIcon

FlagIcon provides flag icons for countries and regions as an attached property that can be applied to Avalonia Image controls.

## Features

- **Sprite-based**: All 270+ flag images are stored in optimized sprite sheets for efficient memory usage
- **Two sizes**: Regular (100x75) and Small (20x15) sprite sheets available
- **Easy to use**: Simple attached property syntax in XAML
- **Lightweight**: Only two sprite sheet images are distributed instead of 270+ individual files

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

### Available Flag Codes

FlagIcon supports the `FlagCode` enum which includes codes for:
- Countries (e.g., CN, US, GB, JP, etc.)
- Regions (e.g., ES_CT for Catalonia, GB_SCT for Scotland)
- Organizations (e.g., EU for European Union, UN for United Nations)

See the `FlagCode` enum in the source code for the complete list of available flags.

## Implementation Details

- Flag icons are loaded from sprite sheets at startup
- Images are cached in memory for efficient reuse
- Uses Avalonia's `CroppedBitmap` to extract individual flags from the sprite sheet
- Attributes on the `FlagCode` enum define the position and size of each flag in the sprite sheets

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
