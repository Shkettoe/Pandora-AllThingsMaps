using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Pandora.Models;

namespace Pandora.ViewModels;

public partial class UnitViewModel(Unit unit, List<Point> points) : ViewModelBase
{
    [ObservableProperty] private Unit _unit = unit;
    [ObservableProperty] private List<Point> _points = points;
    [ObservableProperty] private IImmutableSolidColorBrush _colour = Brushes.LightSteelBlue;
}