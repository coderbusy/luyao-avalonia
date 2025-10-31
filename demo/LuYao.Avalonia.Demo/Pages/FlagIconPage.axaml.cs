using Avalonia.Controls;

namespace LuYao.Avalonia.Demo.Pages;

public partial class FlagIconPage : UserControl
{
    public FlagIconPage()
    {
        InitializeComponent();
        DataContext = new ViewModels.FlagIconViewModel();
    }
}
