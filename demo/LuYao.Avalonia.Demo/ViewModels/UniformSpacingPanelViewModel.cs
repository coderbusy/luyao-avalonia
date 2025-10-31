using System.Collections.ObjectModel;
using Avalonia.Layout;
using ReactiveUI;

namespace LuYao.Avalonia.Demo.ViewModels;

public class UniformSpacingPanelViewModel : ViewModelBase
{
    private bool _isVertical = true;
    private double _spacing = 10.0;

    public ObservableCollection<string> Items { get; }

    public bool IsVertical
    {
        get => _isVertical;
        set
        {
            this.RaiseAndSetIfChanged(ref _isVertical, value);
            this.RaisePropertyChanged(nameof(Orientation));
        }
    }

    public Orientation Orientation => IsVertical ? Orientation.Vertical : Orientation.Horizontal;

    public double Spacing
    {
        get => _spacing;
        set => this.RaiseAndSetIfChanged(ref _spacing, value);
    }

    public UniformSpacingPanelViewModel()
    {
        Items = new ObservableCollection<string>
        {
            "First Item",
            "Second Item",
            "Third Item",
            "Fourth Item",
            "Fifth Item"
        };
    }
}
