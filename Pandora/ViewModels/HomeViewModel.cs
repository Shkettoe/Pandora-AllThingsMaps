using CommunityToolkit.Mvvm.ComponentModel;

namespace Pandora.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    [ObservableProperty] private string _name = "Home";
}