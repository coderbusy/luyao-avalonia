using System.Collections.ObjectModel;
using ReactiveUI;

namespace LuYao.Avalonia.Demo.ViewModels;

public class VirtualizingWrapPanelViewModel : ViewModelBase
{
    public ObservableCollection<ItemViewModel> Items { get; }

    public VirtualizingWrapPanelViewModel()
    {
        Items = new ObservableCollection<ItemViewModel>();
        
        // Create sample items
        for (int i = 0; i < 100; i++)
        {
            Items.Add(new ItemViewModel
            {
                Index = i,
                Text = $"Item {i}",
                IsBreakLine = (i + 1) % 10 == 0 // Items 9, 19, 29, etc. (every 10th item in 0-based indexing)
            });
        }
    }
}

public class ItemViewModel : ReactiveObject
{
    private int _index;
    private string _text = string.Empty;
    private bool _isBreakLine;

    public int Index
    {
        get => _index;
        set => this.RaiseAndSetIfChanged(ref _index, value);
    }

    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }

    public bool IsBreakLine
    {
        get => _isBreakLine;
        set => this.RaiseAndSetIfChanged(ref _isBreakLine, value);
    }
}
