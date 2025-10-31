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
/// Scaling is handled by external controls.
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

    static FlagIcon()
    {
        CodeProperty.Changed.AddClassHandler<Image>(OnCodeChanged);
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

    private static void OnCodeChanged(Image image, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is string code)
        {
            UpdateImage(image, code);
        }
    }

    private static string FormatUri(string code) =>
        $"avares://LuYao.Avalonia.Controls/FlagIcon/Assets/{code.ToLowerInvariant()}.png";

    private static void UpdateImage(Image image, string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            // Use default XX flag if no code is provided
            LoadImage(image, DefaultUri);
            return;
        }

        var uri = new Uri(FormatUri(code));
        if (!AssetLoader.Exists(uri))
        {
            // If code not found, show XX flag
            uri = DefaultUri;
        }

        LoadImage(image, uri);
    }

    private static void LoadImage(Image image, Uri uri)
    {
        try
        {
            using var stream = AssetLoader.Open(uri);
            image.Source = new Bitmap(stream);
        }
        catch
        {
            // If loading fails, clear the source
            image.Source = null;
        }
    }
}
