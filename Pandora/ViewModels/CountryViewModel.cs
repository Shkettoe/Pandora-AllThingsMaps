using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pandora.Models;

namespace Pandora.ViewModels;

public partial class CountryViewModel(Country country, List<Point> points) : ViewModelBase
{
    [ObservableProperty] private Country _country = country;
    // [ObservableProperty] private Polygon _polygon = polygon;
    // [ObservableProperty] private ObservableCollection<Point>  _points = points;
    [ObservableProperty] private List<Point>  _points = points;
    [ObservableProperty] private IImmutableSolidColorBrush _colour = Brushes.LightSteelBlue;

    [RelayCommand]
    private Task ClickEventHandler()
    {
        Colour = Colour.Equals(Brushes.LightSteelBlue) ? Brushes.Yellow : Brushes.LightSteelBlue;
        return Task.CompletedTask;
    }
}