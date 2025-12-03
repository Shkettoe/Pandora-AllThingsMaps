using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
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

    // [ObservableProperty]
    private readonly List<Country> _selectedCountries = [];

    [ObservableProperty] private string _countryName = string.Empty;
    [ObservableProperty] private ObservableCollection<Polygon> _countryPolygons = [];

    [ObservableProperty] private int _canvasWidth = 1000;
    [ObservableProperty] private int _canvasHeight = 500;
    [ObservableProperty] private TranslateTransform _pan = new(1.0, 1.0);
    [ObservableProperty] private ScaleTransform _zoom = new(1.0, 1.0);

    [ObservableProperty] private Point _cursorLocation;

    public MultipleCountriesViewModel()
    {
        _ = LoadCountries();
    }

    private async Task LoadCountries()
    {
        try
        {
            _selectedCountries.Clear();
            _selectedCountries.AddRange(await _geoDataService.GetAllCountries() ??
                                        throw new InvalidOperationException());
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

    public void ChangeScale(double factor, Point center)
    {
        center = new Point(center.X - CanvasWidth / 2.0, center.Y - CanvasHeight / 2.0);
        Zoom.ScaleX *= factor;
        Zoom.ScaleY *= factor;
        Pan.X = center.X - (center.X - Pan.X) * factor;
        Pan.Y = center.Y - (center.Y - Pan.Y) * factor;
    }

    public void PanCanvas(double deltaX, double deltaY)
    {
        Pan.X += deltaX;
        Pan.Y -= deltaY;
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
                var countryToRemove =
                    _selectedCountries.Find(c => c.Name.Equals(CountryName, StringComparison.OrdinalIgnoreCase));
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

    private void RerenderCanvas()
    {
        CountryPolygons.Clear();

        foreach (var transformedPoints in from country in _selectedCountries
                 from polygonPoints in country.GetPolygons()
                 select polygonPoints
                     .Select(coord => new Point(
                         (coord.X + 180) * CanvasWidth / 360,
                         (coord.Y + 90) * CanvasHeight / 180 * -1 + CanvasHeight
                     ))
                     .ToList())
        {
            CountryPolygons.Add(new Polygon
            {
                Points = transformedPoints,
                Fill = Brushes.LightBlue,
                Stroke = Brushes.Black,
                StrokeThickness = 0.1
            });
        }
    }
}