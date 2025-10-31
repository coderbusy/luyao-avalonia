using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace LuYao.Avalonia.Controls;

/// <summary>
/// Provides attached properties for displaying flag icons on Image controls.
/// Flag icons are loaded from sprite sheets for efficient memory usage.
/// </summary>
public class FlagIcon
{
    private static readonly IReadOnlyDictionary<FlagCode, IImage> RegularCache;
    private static readonly IReadOnlyDictionary<FlagCode, IImage> SmallCache;

    /// <summary>
    /// Attached property for setting the flag code
    /// </summary>
    public static readonly AttachedProperty<FlagCode> CodeProperty =
        AvaloniaProperty.RegisterAttached<FlagIcon, Image, FlagCode>("Code");

    /// <summary>
    /// Attached property for using small flag icons (20x15 instead of 100x75)
    /// </summary>
    public static readonly AttachedProperty<bool> UseSmallProperty =
        AvaloniaProperty.RegisterAttached<FlagIcon, Image, bool>("UseSmall", defaultValue: false);

    static FlagIcon()
    {
        var regularDict = new Dictionary<FlagCode, IImage>();
        var smallDict = new Dictionary<FlagCode, IImage>();

        // Load regular sprite sheet
        using (var ms = AssetLoader.Open(new Uri("avares://LuYao.Avalonia.Controls/Assets/Images/flags-sprite.png")))
        {
            var bmp = new Bitmap(ms);
            var t = typeof(FlagCode);
            foreach (var code in Enum.GetValues(typeof(FlagCode)))
            {
                var flagCode = (FlagCode)code;
                string name = flagCode.ToString();
                FieldInfo? field = t.GetField(name);
                if (field == null) continue;
                
                var attr = field.GetCustomAttribute<FlagIconRectangleAttribute>();
                if (attr == null) continue;
                
                var croppedBitmap = new CroppedBitmap(bmp, new PixelRect(attr.X, attr.Y, attr.Width, attr.Height));
                regularDict[flagCode] = croppedBitmap;
            }
        }

        // Load small sprite sheet
        using (var ms = AssetLoader.Open(new Uri("avares://LuYao.Avalonia.Controls/Assets/Images/flags-sprite-small.png")))
        {
            var bmp = new Bitmap(ms);
            var t = typeof(FlagCode);
            foreach (var code in Enum.GetValues(typeof(FlagCode)))
            {
                var flagCode = (FlagCode)code;
                string name = flagCode.ToString();
                FieldInfo? field = t.GetField(name);
                if (field == null) continue;
                
                var attr = field.GetCustomAttribute<FlagIconSmallRectangleAttribute>();
                if (attr == null) continue;
                
                var croppedBitmap = new CroppedBitmap(bmp, new PixelRect(attr.X, attr.Y, attr.Width, attr.Height));
                smallDict[flagCode] = croppedBitmap;
            }
        }

        RegularCache = regularDict;
        SmallCache = smallDict;

        CodeProperty.Changed.AddClassHandler<Image>(OnCodeChanged);
        UseSmallProperty.Changed.AddClassHandler<Image>(OnUseSmallChanged);
    }

    /// <summary>
    /// Gets the flag code for the image
    /// </summary>
    public static FlagCode GetCode(Image image)
    {
        return image.GetValue(CodeProperty);
    }

    /// <summary>
    /// Sets the flag code for the image
    /// </summary>
    public static void SetCode(Image image, FlagCode value)
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
        if (e.NewValue is FlagCode code)
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

    private static void UpdateImage(Image image, FlagCode code, bool useSmall)
    {
        var cache = useSmall ? SmallCache : RegularCache;
        if (cache.TryGetValue(code, out var img))
        {
            image.Source = img;
        }
    }
}
