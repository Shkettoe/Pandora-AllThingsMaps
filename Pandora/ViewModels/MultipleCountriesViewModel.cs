using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pandora.Models;
using Pandora.Services;

namespace Pandora.ViewModels;

public partial class MultipleCountriesViewModel : ViewModelBase
{
    private readonly GeoDataService _geoDataService = new();

    private readonly List<Country> _selectedCountries = [];

    [ObservableProperty] private string _countryName = string.Empty;

    [ObservableProperty] private ObservableCollection<Polygon> _countryPolygons = [];

    [ObservableProperty] private int _canvasWidth = 1000;
    [ObservableProperty] private int _canvasHeight = 600;
    [ObservableProperty] private double _scale = 10;
    [ObservableProperty] private double _offsetX;
    [ObservableProperty] private double _offsetY;

    public MultipleCountriesViewModel()
    {
        CountryName = "France";
        _ = ToggleCountry();
        CountryName = "Germany";
        _ = ToggleCountry();
        CountryName = "Italy";
        _ = ToggleCountry();
    }

    [RelayCommand]
    private Task ChangeScale(string direction)
    {
        Scale += double.Parse(direction);
        ResetPositions();
        return Task.CompletedTask;
    }

    public void PanCanvas(double deltaX, double deltaY)
    {
        OffsetX += deltaX;
        OffsetY += deltaY;
        ResetPositions();
    }

    [RelayCommand]
    private async Task ToggleCountry()
    {
        try
        {
            if (!_selectedCountries.Exists(c => c.Name.Equals(CountryName, StringComparison.OrdinalIgnoreCase)))
            {
                var country = await _geoDataService.GetCountry(CountryName);
                if (country != null) _selectedCountries.Add(country);
            }
            else
            {
                var countryToRemove = _selectedCountries.Find(c => c.Name.Equals(CountryName, StringComparison.OrdinalIgnoreCase));
                if (countryToRemove != null)
                {
                    _selectedCountries.Remove(countryToRemove);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            RerenderCanvas();
        }
    }

    public void RerenderCanvas()
    {
        CountryPolygons.Clear();

        // Calculate offset to center the polygon
        foreach (var points in _selectedCountries.Select(sc => new List<Point>(from coord in sc.GetPolygonPoints()
                     let x = coord.X * Scale + OffsetX
                     let y = CanvasHeight - (coord.Y * Scale + OffsetY)
                     select new Point(x, y))))
        {
            CountryPolygons.Add(new Polygon
                { Points = points, Fill = Brushes.LightBlue, Stroke = Brushes.Black, StrokeThickness = 1 });
        }
    }

    public void ResetPositions()
    {
        for (var i = 0; i < _selectedCountries.Count; i++)
        {
            var points = _selectedCountries[i].GetPolygonPoints()
                .Select(coord => new Point(
                    coord.X * Scale + OffsetX,
                    CanvasHeight - (coord.Y * Scale + OffsetY)))
                .ToList();
        
            CountryPolygons[i].Points = points;
        }
    }
}