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
        for (int i = 0; i < 1000; i++)
        {
            Items.Add(new ItemViewModel
            {
                Index = i,
                Text = $"Item {i}"
            });
        }
    }
}

public class ItemViewModel : ReactiveObject
{
    private int _index;
    private string _text = string.Empty;

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
}
