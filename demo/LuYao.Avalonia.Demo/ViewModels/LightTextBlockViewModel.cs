using ReactiveUI;

namespace LuYao.Avalonia.Demo.ViewModels;

public class LightTextBlockViewModel : ViewModelBase
{
    private string _sampleText = "Hello, LightTextBlock!";
    private double _fontSize = 24.0;
    private string _selectedColor = "Black";

    public string SampleText
    {
        get => _sampleText;
        set => this.RaiseAndSetIfChanged(ref _sampleText, value);
    }

    public double FontSize
    {
        get => _fontSize;
        set => this.RaiseAndSetIfChanged(ref _fontSize, value);
    }

    public string SelectedColor
    {
        get => _selectedColor;
        set => this.RaiseAndSetIfChanged(ref _selectedColor, value);
    }

    public string[] AvailableColors { get; } = 
    [
        "Black",
        "Red",
        "Blue",
        "Green",
        "Purple",
        "Orange"
    ];
}
