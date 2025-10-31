using Avalonia.Controls;
using LuYao.Avalonia.Demo.ViewModels;

namespace LuYao.Avalonia.Demo.Pages;

public partial class VirtualizingWrapPanelPage : UserControl
{
    public VirtualizingWrapPanelPage()
    {
        InitializeComponent();
        DataContext = new VirtualizingWrapPanelViewModel();
    }
}
