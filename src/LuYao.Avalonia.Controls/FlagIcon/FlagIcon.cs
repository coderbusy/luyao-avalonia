using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace LuYao.Avalonia.Controls;

/// <summary>
/// Provides attached properties for displaying flag icons on Image controls.
/// Flag icons are loaded from individual PNG files.
/// AOT-compatible implementation without reflection.
/// </summary>
public class FlagIcon
{
    private static readonly Uri DefaultUri = new("avares://LuYao.Avalonia.Controls/FlagIcon/Assets/xx.png");

    /// <summary>
    /// Attached property for setting the flag code (string)
    /// </summary>
    public static readonly AttachedProperty<string> CodeProperty =
        AvaloniaProperty.RegisterAttached<FlagIcon, Image, string>("Code", defaultValue: string.Empty);

    /// <summary>
    /// Attached property for using small flag icons (thumbnails)
    /// When true, images are scaled down for better performance
    /// </summary>
    public static readonly AttachedProperty<bool> UseSmallProperty =
        AvaloniaProperty.RegisterAttached<FlagIcon, Image, bool>("UseSmall", defaultValue: false);

    static FlagIcon()
    {
        CodeProperty.Changed.AddClassHandler<Image>(OnCodeChanged);
        UseSmallProperty.Changed.AddClassHandler<Image>(OnUseSmallChanged);
    }

    /// <summary>
    /// Gets the flag code for the image
    /// </summary>
    public static string GetCode(Image image)
    {
        return image.GetValue(CodeProperty);
    }

    /// <summary>
    /// Sets the flag code for the image
    /// </summary>
    public static void SetCode(Image image, string value)
    {
        image.SetValue(CodeProperty, value);
    }

    /// <summary>
    /// Gets whether to use small flag icons
    /// </summary>
    public static bool GetUseSmall(Image image)
    {
        return image.GetValue(UseSmallProperty);
    }

    /// <summary>
    /// Sets whether to use small flag icons
    /// </summary>
    public static void SetUseSmall(Image image, bool value)
    {
        image.SetValue(UseSmallProperty, value);
    }

    private static void OnCodeChanged(Image image, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is string code)
        {
            UpdateImage(image, code, GetUseSmall(image));
        }
    }

    private static void OnUseSmallChanged(Image image, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool useSmall)
        {
            UpdateImage(image, GetCode(image), useSmall);
        }
    }

    private static string FormatUri(string code) =>
        $"avares://LuYao.Avalonia.Controls/FlagIcon/Assets/{code.ToLowerInvariant()}.png";

    private static void UpdateImage(Image image, string code, bool useSmall)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            // Use default XX flag if no code is provided
            LoadImage(image, DefaultUri, useSmall);
            return;
        }

        var uri = new Uri(FormatUri(code));
        if (!AssetLoader.Exists(uri))
        {
            // If code not found, show XX flag
            uri = DefaultUri;
        }

        LoadImage(image, uri, useSmall);
    }

    private static void LoadImage(Image image, Uri uri, bool useSmall)
    {
        try
        {
            var scaling = TopLevel.GetTopLevel(image)?.RenderScaling ?? 1.0;
            
            using var stream = AssetLoader.Open(uri);
            
            if (useSmall)
            {
                // For small/thumbnail mode, decode to a smaller size
                // Default small size is 20x15 (scaled based on display DPI)
                var targetWidth = (int)(20 * scaling);
                image.Source = Bitmap.DecodeToWidth(stream, targetWidth);
            }
            else
            {
                // For regular mode, use the full size image with DPI scaling
                var targetWidth = (int)(100 * scaling);
                image.Source = Bitmap.DecodeToWidth(stream, targetWidth);
            }
        }
        catch
        {
            // If loading fails, clear the source
            image.Source = null;
        }
    }
}
