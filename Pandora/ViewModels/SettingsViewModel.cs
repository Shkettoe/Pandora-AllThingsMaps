using CommunityToolkit.Mvvm.ComponentModel;

namespace Pandora.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty] private string _name = "Settings";
}