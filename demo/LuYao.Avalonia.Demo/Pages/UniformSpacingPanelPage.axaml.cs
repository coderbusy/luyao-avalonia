using Avalonia.Controls;
using LuYao.Avalonia.Demo.ViewModels;

namespace LuYao.Avalonia.Demo.Pages;

public partial class UniformSpacingPanelPage : UserControl
{
    public UniformSpacingPanelPage()
    {
        InitializeComponent();
        DataContext = new UniformSpacingPanelViewModel();
    }
}
