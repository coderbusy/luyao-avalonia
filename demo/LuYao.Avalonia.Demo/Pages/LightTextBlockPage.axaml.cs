using Avalonia.Controls;

namespace LuYao.Avalonia.Demo.Pages;

public partial class LightTextBlockPage : UserControl
{
    public LightTextBlockPage()
    {
        InitializeComponent();
        DataContext = new ViewModels.LightTextBlockViewModel();
    }
}
