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

    [ObservableProperty] private string _countryName = "";

    [ObservableProperty] private ObservableCollection<Polygon> _countryPolygons = [];
    
    [ObservableProperty] private int _canvasWidth = 1000;
    [ObservableProperty] private int _canvasHeight = 600;

    public MultipleCountriesViewModel()
    {
        var points = new ObservableCollection<Point>([new Point(x: 1, y:1), new Point(x:900, y:500), new Point(x: 900, y: 1)]);
        var polygon = new Polygon { Points = points, Fill = Brushes.LightBlue, Stroke = Brushes.Black, StrokeThickness = 1};
        CountryPolygons.Add(polygon);
    }

    [RelayCommand]
    private async Task ToggleCountry()
    {
        try
        {
            if (!_selectedCountries.Exists(c => c.Name.Equals(CountryName)))
            {
                Console.WriteLine($"Trying to get a country with the name {CountryName}");
                var country = await _geoDataService.GetCountry(CountryName);
                if (country != null) _selectedCountries.Add(country);
            }
            else
            {
                _selectedCountries.Remove(_selectedCountries.Find(c => c.Name.Equals(CountryName)) ??
                                          throw new InvalidOperationException());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally{ResetCanvas();}
    }

    private void ResetCanvas()
    {
        CountryPolygons.Clear();
        CountryName = string.Empty;

        // bounds for scaling
        var allPoints = _selectedCountries.SelectMany(sc => sc.GetPolygonPoints()).ToList();
        var minX = allPoints.Min(p => p.X);
        var minY = allPoints.Min(p => p.Y);
        var maxX = allPoints.Max(p => p.X);
        var maxY = allPoints.Max(p => p.Y);

        var width = maxX - minX;
        var height = maxY - minY;

        const int padding = 50;
        var scaleX = (CanvasWidth - padding * 2) / width;
        var scaleY = (CanvasHeight - padding * 2) / height;
        var scale = Math.Min(scaleX, scaleY); // Keep aspect ratio

        // Calculate offset to center the polygon
        var offsetX = (CanvasWidth - width * scale) / 2 - minX * scale;
        var offsetY = (CanvasHeight - height * scale) / 2 - minY * scale;

        ObservableCollection<Polygon> polygons = [];
        foreach (var sc in _selectedCountries)
        {
            List<Point> points = [];
            foreach (var coord in sc.GetPolygonPoints())
            {
                var x = coord.X * scale + offsetX;
                var y = CanvasHeight - (coord.Y * scale + offsetY); // Flip Y axis for screen coordinates
                if(x > CanvasWidth || y > CanvasHeight || x <  0 || y < 0) Console.WriteLine($"{x}, {y}");
                points.Add(new Point(x, y));
            }

            var polygon = new Polygon
                { Points = points, Fill = Brushes.LightBlue, Stroke = Brushes.Black, StrokeThickness = 1 };
            polygons.Add(polygon);
        }
        CountryPolygons = polygons;        
    }
}