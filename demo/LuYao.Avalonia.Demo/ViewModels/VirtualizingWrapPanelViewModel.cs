using System.Collections.ObjectModel;
using Avalonia;
using LuYao.Avalonia.Controls;
using ReactiveUI;

namespace LuYao.Avalonia.Demo.ViewModels;

public class VirtualizingWrapPanelViewModel : ViewModelBase
{
    private int _itemCount = 10000;
    private bool _useVariableSizes = false;
    
    public ObservableCollection<ItemViewModel> Items { get; }

    public int ItemCount
    {
        get => _itemCount;
        set => this.RaiseAndSetIfChanged(ref _itemCount, value);
    }

    public bool UseVariableSizes
    {
        get => _useVariableSizes;
        set
        {
            this.RaiseAndSetIfChanged(ref _useVariableSizes, value);
            this.RaisePropertyChanged(nameof(ItemSizeProvider));
        }
    }

    public IItemSizeProvider? ItemSizeProvider => UseVariableSizes ? new VariableItemSizeProvider() : null;

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

/// <summary>
/// Example ItemSizeProvider that provides variable sizes for items.
/// Even items are smaller, odd items are larger.
/// </summary>
public class VariableItemSizeProvider : IItemSizeProvider
{
    public Size GetSizeForItem(int itemIndex)
    {
        // Return different sizes based on index
        // Even items: 100x80
        // Odd items: 150x120
        return itemIndex % 2 == 0 
            ? new Size(100, 80) 
            : new Size(150, 120);
    }
}
