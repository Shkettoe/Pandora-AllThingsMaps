using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using FluentIcons.Common;

namespace Pandora.ViewModels;

public partial class SidebarViewModel : ViewModelBase
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(Pandora), nameof(SideBarWidth), nameof(HamburgerIcon))] private bool _hamburger = true;

    public string Pandora => Hamburger ? "Pandora" : "P";

    public double SideBarWidth => Hamburger ? 240.0 : 60.0;
    
    public string HamburgerIcon => Hamburger ? "ChevronLeft" : "ChevronRight";

    [RelayCommand]
    private void ResizeSidebar()
    {
        Hamburger = !Hamburger;
    }
}