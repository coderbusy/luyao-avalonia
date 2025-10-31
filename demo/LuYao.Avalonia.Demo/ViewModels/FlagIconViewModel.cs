using ReactiveUI;

namespace LuYao.Avalonia.Demo.ViewModels;

public class FlagIconViewModel : ViewModelBase
{
    private string _selectedCode = "CN";
    private double _flagWidth = 100;

    public string SelectedCode
    {
        get => _selectedCode;
        set => this.RaiseAndSetIfChanged(ref _selectedCode, value);
    }

    public double FlagWidth
    {
        get => _flagWidth;
        set => this.RaiseAndSetIfChanged(ref _flagWidth, value);
    }

    public string[] PopularCountryCodes { get; } = 
    [
        "CN", "US", "GB", "JP", "FR", "DE", "IT", "ES", "CA", "AU",
        "IN", "BR", "RU", "KR", "MX", "AR", "ZA", "NL", "SE", "CH"
    ];
}
