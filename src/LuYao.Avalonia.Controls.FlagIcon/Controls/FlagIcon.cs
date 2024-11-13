using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace LuYao.Avalonia.Controls;

public class FlagIcon : global::Avalonia.Controls.Image
{
    public FlagIcon()
    {
        this.Width = 22;
    }

    private static readonly Uri DefaultUri = new("avares://LuYao.Avalonia.Controls.FlagIcon/Assets/xx.png");

    public static readonly StyledProperty<string> CodeProperty =
        AvaloniaProperty.Register<FlagIcon, string>(
            nameof(Code),
            defaultBindingMode: BindingMode.OneWay,
            defaultValue: string.Empty
        );

    public string Code
    {
        get => GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }

    private static string Format(string abbr) =>
        $"avares://LuYao.Avalonia.Controls.FlagIcon/Assets/{abbr.ToLowerInvariant()}.png";

    private string _last = string.Empty;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CodeProperty || change.Property == WidthProperty || change.Property == HeightProperty)
        {
            this.ResetImage();
        }
    }

    private void ResetImage()
    {
        var url = string.IsNullOrWhiteSpace(this.Code) ? DefaultUri : new Uri(Format(this.Code));
        if (!AssetLoader.Exists(url)) url = DefaultUri;
        var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;
        double w = 100, h = 75;
        if (this.Width > 0) w = this.Width;
        if (this.Height > 0) h = this.Height;
        var k = string.Join("x", this.Code, w.ToString("0"), h.ToString("0"));
        if (_last == k) return;
        var v = w;
        var forHeight = false;
        if (h > w)
        {
            forHeight = true;
            v = h;
        }

        v *= scaling;

        using var ms = AssetLoader.Open(url);
        this.Source = forHeight
            ? Bitmap.DecodeToHeight(ms, Convert.ToInt32(v))
            : Bitmap.DecodeToWidth(ms, Convert.ToInt32(v));
        this._last = k;
    }
}
