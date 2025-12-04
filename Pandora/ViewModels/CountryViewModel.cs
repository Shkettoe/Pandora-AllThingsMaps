using System.Collections.Generic;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Pandora.Models;

namespace Pandora.ViewModels;

public partial class CountryViewModel(Country country, Polygon polygon) : ViewModelBase
{
    [ObservableProperty] private Country _country = country;
    [ObservableProperty] private Polygon _polygon = polygon;
}