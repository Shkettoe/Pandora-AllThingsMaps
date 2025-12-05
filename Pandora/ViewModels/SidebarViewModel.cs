using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pandora.Data.Enums;
using Pandora.Factories;

namespace Pandora.ViewModels;

public partial class SidebarViewModel : ViewModelBase
{
    private readonly PageFactory _pageFactory;    
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Pandora), nameof(SideBarWidth), nameof(HamburgerIcon))]
    private bool _hamburger = true;

    public string Pandora => Hamburger ? "Pandora" : "P";

    public double SideBarWidth => Hamburger ? 240.0 : 60.0;

    public string HamburgerIcon => Hamburger ? "ChevronLeft" : "ChevronRight";

    [ObservableProperty]
    private ViewModelBase? _selectedViewModel;

    public SidebarViewModel(PageFactory pageFactory)
    {
        _pageFactory = pageFactory;
        // Only navigate if not in design mode
        if (!Design.IsDesignMode)
        {
            GoTo(PageNamesEnum.Default);
        }
    }
    public SidebarViewModel() : this(null!) {}
    
    [RelayCommand]
    private void ResizeSidebar()
    {
        Hamburger = !Hamburger;
    }

    [RelayCommand]
    private void GoTo(PageNamesEnum page)
    {
        SelectedViewModel = _pageFactory.Create(page);
    }
}