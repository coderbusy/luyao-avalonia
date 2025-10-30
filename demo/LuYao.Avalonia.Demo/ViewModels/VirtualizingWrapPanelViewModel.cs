using System.Collections.ObjectModel;
using ReactiveUI;

namespace LuYao.Avalonia.Demo.ViewModels;

public class VirtualizingWrapPanelViewModel : ViewModelBase
{
    private int _itemCount = 10000;
    
    public ObservableCollection<ItemViewModel> Items { get; }

    public int ItemCount
    {
        get => _itemCount;
        set => this.RaiseAndSetIfChanged(ref _itemCount, value);
    }

    public VirtualizingWrapPanelViewModel()
    {
        Items = new ObservableCollection<ItemViewModel>();
        
        // Create sample items - testing with large dataset for virtualization
        for (int i = 0; i < ItemCount; i++)
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
