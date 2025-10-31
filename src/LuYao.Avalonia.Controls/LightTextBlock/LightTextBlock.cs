using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace LuYao.Avalonia.Controls;

/// <summary>
/// A lightweight control for displaying text with limited properties.
/// Does not support advanced features like soft wrapping.
/// </summary>
public class LightTextBlock : Control
{
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<LightTextBlock, string>(nameof(Text), string.Empty);

    public static readonly StyledProperty<FontFamily?> FontFamilyProperty =
        AvaloniaProperty.Register<LightTextBlock, FontFamily?>(nameof(FontFamily));

    public static readonly StyledProperty<double> FontSizeProperty =
        AvaloniaProperty.Register<LightTextBlock, double>(nameof(FontSize), 12.0);

    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        AvaloniaProperty.Register<LightTextBlock, IBrush?>(nameof(Foreground), Brushes.Black);

    static LightTextBlock()
    {
        AffectsRender<LightTextBlock>(TextProperty, FontFamilyProperty, FontSizeProperty, ForegroundProperty);
        AffectsMeasure<LightTextBlock>(TextProperty, FontFamilyProperty, FontSizeProperty);
    }

    /// <summary>
    /// Gets or sets the text to display.
    /// </summary>
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets the font family.
    /// </summary>
    public FontFamily? FontFamily
    {
        get => GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground brush.
    /// </summary>
    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    private FormattedText GetFormattedText()
    {
        if (string.IsNullOrEmpty(Text))
            return new FormattedText(
                string.Empty,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                Typeface.Default,
                FontSize,
                Brushes.Black);

        var typeface = new Typeface(FontFamily ?? Typeface.Default.FontFamily);
        var foreground = Foreground ?? Brushes.Black;

        return new FormattedText(
            Text,
            CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight,
            typeface,
            FontSize,
            foreground);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (string.IsNullOrEmpty(Text))
            return;

        var formattedText = GetFormattedText();
        context.DrawText(formattedText, new Point(0, 0));
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (string.IsNullOrEmpty(Text))
            return new Size(0, 0);

        var formattedText = GetFormattedText();
        return new Size(formattedText.Width, formattedText.Height);
    }
}
