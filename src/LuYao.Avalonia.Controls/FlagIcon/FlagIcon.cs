using System;
using System.Collections.Generic;
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
        var regularDict = new Dictionary<string, IImage>(StringComparer.OrdinalIgnoreCase);
        var smallDict = new Dictionary<string, IImage>(StringComparer.OrdinalIgnoreCase);

        // Load regular sprite sheet
        using (var ms = AssetLoader.Open(new Uri("avares://LuYao.Avalonia.Controls/Assets/Images/flags-sprite.png")))
        {
            var bmp = new Bitmap(ms);
            LoadFlags(bmp, regularDict, false);
            
            // Get XX flag as default
            var xxRect = FlagData.GetRegularRect("XX");
            if (xxRect.HasValue)
            {
                DefaultRegularImage = new CroppedBitmap(bmp, xxRect.Value);
            }
        }

        // Load small sprite sheet
        using (var ms = AssetLoader.Open(new Uri("avares://LuYao.Avalonia.Controls/Assets/Images/flags-sprite-small.png")))
        {
            var bmp = new Bitmap(ms);
            LoadFlags(bmp, smallDict, true);
            
            // Get XX flag as default
            var xxRect = FlagData.GetSmallRect("XX");
            if (xxRect.HasValue)
            {
                DefaultSmallImage = new CroppedBitmap(bmp, xxRect.Value);
            }
        }

        RegularCache = regularDict;
        SmallCache = smallDict;

        CodeProperty.Changed.AddClassHandler<Image>(OnCodeChanged);
        UseSmallProperty.Changed.AddClassHandler<Image>(OnUseSmallChanged);
    }

    private static void LoadFlags(Bitmap sprite, Dictionary<string, IImage> cache, bool isSmall)
    {
        // Get all flag codes and their rectangles from FlagData
        // This is AOT-compatible as it doesn't use reflection
        var testCodes = new[]
        {
            "AD", "AE", "AF", "AG", "AI", "AL", "AM", "AO", "AQ", "AR", "ARAB", "AS", "AT", "AU", "AW", "AX", "AZ",
            "BA", "BB", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BL", "BM", "BN", "BO", "BQ", "BR", "BS", "BT", "BV", "BW", "BY", "BZ",
            "CA", "CC", "CD", "CEFTA", "CF", "CG", "CH", "CI", "CK", "CL", "CM", "CN", "CO", "CP", "CR", "CU", "CV", "CW", "CX", "CY", "CZ",
            "DE", "DG", "DJ", "DK", "DM", "DO", "DZ",
            "EAC", "EC", "EE", "EG", "EH", "ER", "ES", "ES-CT", "ES-GA", "ES-PV", "ET", "EU",
            "FI", "FJ", "FK", "FM", "FO", "FR",
            "GA", "GB", "GB-ENG", "GB-NIR", "GB-SCT", "GB-WLS", "GD", "GE", "GF", "GG", "GH", "GI", "GL", "GM", "GN", "GP", "GQ", "GR", "GS", "GT", "GU", "GW", "GY",
            "HK", "HM", "HN", "HR", "HT", "HU",
            "IC", "ID", "IE", "IL", "IM", "IN", "IO", "IQ", "IR", "IS", "IT",
            "JE", "JM", "JO", "JP",
            "KE", "KG", "KH", "KI", "KM", "KN", "KP", "KR", "KW", "KY", "KZ",
            "LA", "LB", "LC", "LI", "LK", "LR", "LS", "LT", "LU", "LV", "LY",
            "MA", "MC", "MD", "ME", "MF", "MG", "MH", "MK", "ML", "MM", "MN", "MO", "MP", "MQ", "MR", "MS", "MT", "MU", "MV", "MW", "MX", "MY", "MZ",
            "NA", "NC", "NE", "NF", "NG", "NI", "NL", "NO", "NP", "NR", "NU", "NZ",
            "OM",
            "PA", "PE", "PF", "PG", "PH", "PK", "PL", "PM", "PN", "PR", "PS", "PT", "PW", "PY",
            "QA",
            "RE", "RO", "RS", "RU", "RW",
            "SA", "SB", "SC", "SD", "SE", "SG", "SH", "SH-AC", "SH-HL", "SH-TA", "SI", "SJ", "SK", "SL", "SM", "SN", "SO", "SR", "SS", "ST", "SV", "SX", "SY", "SZ",
            "TC", "TD", "TF", "TG", "TH", "TJ", "TK", "TL", "TM", "TN", "TO", "TR", "TT", "TV", "TW", "TZ",
            "UA", "UG", "UM", "UN", "US", "UY", "UZ",
            "VA", "VC", "VE", "VG", "VI", "VN", "VU",
            "WF", "WS",
            "XK", "XX",
            "YE", "YT",
            "ZA", "ZM", "ZW"
        };

        foreach (var code in testCodes)
        {
            var rect = isSmall ? FlagData.GetSmallRect(code) : FlagData.GetRegularRect(code);
            if (rect.HasValue)
            {
                var croppedBitmap = new CroppedBitmap(sprite, rect.Value);
                cache[code] = croppedBitmap;
            }
        }
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
