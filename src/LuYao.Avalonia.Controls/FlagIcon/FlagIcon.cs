using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace LuYao.Avalonia.Controls;

/// <summary>
/// Provides attached properties for displaying flag icons on Image controls.
/// Flag icons are loaded from sprite sheets for efficient memory usage.
/// AOT-compatible implementation without reflection.
/// </summary>
public class FlagIcon
{
    private static readonly IReadOnlyDictionary<string, IImage> RegularCache;
    private static readonly IReadOnlyDictionary<string, IImage> SmallCache;
    private static readonly IImage? DefaultRegularImage;
    private static readonly IImage? DefaultSmallImage;

    /// <summary>
    /// Attached property for setting the flag code (string)
    /// </summary>
    public static readonly AttachedProperty<string> CodeProperty =
        AvaloniaProperty.RegisterAttached<FlagIcon, Image, string>("Code", defaultValue: string.Empty);

    /// <summary>
    /// Attached property for using small flag icons (20x15 instead of 100x75)
    /// </summary>
    public static readonly AttachedProperty<bool> UseSmallProperty =
        AvaloniaProperty.RegisterAttached<FlagIcon, Image, bool>("UseSmall", defaultValue: false);

    static FlagIcon()
    {
        // Load regular sprite sheet
        using (var ms = AssetLoader.Open(new Uri("avares://LuYao.Avalonia.Controls/Assets/Images/flags-sprite.png")))
        {
            var bmp = new Bitmap(ms);
            var flags = FlagData.GetRegularFlags().ToList();
            
            var regularDict = new Dictionary<string, IImage>(StringComparer.OrdinalIgnoreCase);
            foreach (var (code, rect) in flags)
            {
                regularDict[code] = new CroppedBitmap(bmp, rect);
            }
            RegularCache = regularDict;
            
            // Set default XX flag
            if (regularDict.TryGetValue("XX", out var defaultImg))
            {
                DefaultRegularImage = defaultImg;
            }
        }

        // Load small sprite sheet
        using (var ms = AssetLoader.Open(new Uri("avares://LuYao.Avalonia.Controls/Assets/Images/flags-sprite-small.png")))
        {
            var bmp = new Bitmap(ms);
            var flags = FlagData.GetSmallFlags().ToList();
            
            var smallDict = new Dictionary<string, IImage>(StringComparer.OrdinalIgnoreCase);
            foreach (var (code, rect) in flags)
            {
                smallDict[code] = new CroppedBitmap(bmp, rect);
            }
            SmallCache = smallDict;
            
            // Set default XX flag
            if (smallDict.TryGetValue("XX", out var defaultImg))
            {
                DefaultSmallImage = defaultImg;
            }
        }

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

    private static void UpdateImage(Image image, string code, bool useSmall)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            // Use default XX flag if no code is provided
            image.Source = useSmall ? DefaultSmallImage : DefaultRegularImage;
            return;
        }

        var cache = useSmall ? SmallCache : RegularCache;
        if (cache.TryGetValue(code, out var img))
        {
            image.Source = img;
        }
        else
        {
            // If code not found, show XX flag
            image.Source = useSmall ? DefaultSmallImage : DefaultRegularImage;
        }
    }
}
