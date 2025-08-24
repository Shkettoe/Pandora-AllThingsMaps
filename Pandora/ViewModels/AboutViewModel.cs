using CommunityToolkit.Mvvm.ComponentModel;

namespace Pandora.ViewModels;

public partial class AboutViewModel : ViewModelBase
{
    [ObservableProperty] private string _name = "About";
}