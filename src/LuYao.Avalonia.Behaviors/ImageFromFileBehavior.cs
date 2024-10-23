using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Xaml.Interactions.Custom;
using System;
using System.IO;
using System.Reactive.Disposables;

namespace LuYao.Avalonia.Behaviors;

public class ImageFromFileBehavior : DisposingBehavior<Image>
{
    public static readonly StyledProperty<String?> SourceProperty = AvaloniaProperty.Register<ImageFromFileBehavior, String?>(nameof(Source));

    public String? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    protected override void OnAttached(CompositeDisposable disposables)
    {
        var image = this.AssociatedObject;
        if (image is not null) this.UpdateImage(image);
    }

    public int MaxWidth { get; set; } = int.MaxValue;
    public int MaxHeight { get; set; } = int.MaxValue;

    private void UpdateImage(Image image)
    {
        var source = this.Source;
        if (String.IsNullOrWhiteSpace(source))
        {
            image.Source = default;
        }
        else if (!File.Exists(source))
        {
            image.Source = default;
        }
        else
        {
            if (IsValid(this.MaxWidth) && IsValid(this.MaxHeight))
            {
                bool forWidth = (this.MaxWidth >= this.MaxHeight);
                using var fs = File.OpenRead(source);
                image.Source = forWidth
                    ? Bitmap.DecodeToWidth(fs, this.MaxWidth)
                    : Bitmap.DecodeToHeight(fs, this.MaxHeight);
            }
            else if (IsValid(this.MaxWidth))
            {
                using var fs = File.OpenRead(source);
                image.Source = Bitmap.DecodeToWidth(fs, this.MaxWidth);
            }
            else if (IsValid(this.MaxHeight))
            {
                using var fs = File.OpenRead(source);
                image.Source = Bitmap.DecodeToHeight(fs, this.MaxHeight);
            }
            else
            {
                image.Source = new Bitmap(source);
            }
        }
    }

    private static bool IsValid(int value)
    {
        return value is > 0 and < int.MaxValue;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SourceProperty)
        {
            var image = this.AssociatedObject;
            if (image is not null) this.UpdateImage(image);
        }
    }
}
