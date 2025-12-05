using CommunityToolkit.Mvvm.ComponentModel;

namespace Pandora.ViewModels;

public partial class MainViewModel(SidebarViewModel sidebarViewModel) : ViewModelBase
{
    
    public SidebarViewModel SidebarViewModel { get; } = sidebarViewModel;

    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia UI!";
    
    public MainViewModel() : this(new SidebarViewModel()){}
}
