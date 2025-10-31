namespace LuYao.Avalonia.Demo.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";
    
    public VirtualizingWrapPanelViewModel VirtualizingWrapPanelViewModel { get; } = new();
}
