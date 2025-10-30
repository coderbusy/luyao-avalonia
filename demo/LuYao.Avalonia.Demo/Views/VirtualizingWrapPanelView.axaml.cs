using Avalonia.Controls;
using LuYao.Avalonia.Demo.ViewModels;

namespace LuYao.Avalonia.Demo.Views;

public partial class VirtualizingWrapPanelView : UserControl
{
    public VirtualizingWrapPanelView()
    {
        InitializeComponent();
        this.DataContext = new VirtualizingWrapPanelViewModel();
    }
}
