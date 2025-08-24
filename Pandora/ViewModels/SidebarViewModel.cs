using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using FluentIcons.Common;

namespace Pandora.ViewModels;

public partial class SidebarViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Pandora), nameof(SideBarWidth), nameof(HamburgerIcon))]
    private bool _hamburger = true;

    public string Pandora => Hamburger ? "Pandora" : "P";

    public double SideBarWidth => Hamburger ? 240.0 : 60.0;

    public string HamburgerIcon => Hamburger ? "ChevronLeft" : "ChevronRight";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentPageName))]
    private ViewModelBase? _selectedViewModel;

    private readonly HomeViewModel _homeViewModel = new();
    private readonly AboutViewModel _aboutViewModel = new();
    private readonly SettingsViewModel _settingsViewModel = new();

    public SidebarViewModel()
    {
        SelectedViewModel = _homeViewModel;
    }

    [RelayCommand]
    private void ResizeSidebar()
    {
        Hamburger = !Hamburger;
    }

    [RelayCommand]
    private void GoTo(string page)
    {
        SelectedViewModel = page switch
        {
            "Home" => _homeViewModel,
            "About" => _aboutViewModel,
            "Settings" => _settingsViewModel,
            _ => _homeViewModel
        };
    }

    // Current page name for display
    public string CurrentPageName => SelectedViewModel switch
    {
        HomeViewModel => "Home",
        AboutViewModel => "About",
        SettingsViewModel => "Settings",
        _ => "Home"
    };
}