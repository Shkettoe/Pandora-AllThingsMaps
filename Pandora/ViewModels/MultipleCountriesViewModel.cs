using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
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
    [ObservableProperty] private ObservableCollection<CountryViewModel> _countries = [];

    [ObservableProperty] private int _canvasWidth = 1000;
    [ObservableProperty] private int _canvasHeight = 500;
    [ObservableProperty] private TranslateTransform _pan = new(0.0, 0.0);
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
        var (x, y) = new Point(center.X - CanvasWidth / 2.0, center.Y - CanvasHeight / 2.0);
        if (Zoom.ScaleX * factor > 200 || Zoom.ScaleY * factor > 200 || Zoom.ScaleX * factor < 1 ||
            Zoom.ScaleY * factor < 1) return;
        Zoom.ScaleX *= factor;
        Zoom.ScaleY *= factor;
        Pan.X = x - (x - Pan.X) * factor;
        Pan.Y = y - (y - Pan.Y) * factor;
    }

    public void PanCanvas(double deltaX, double deltaY)
    {
        if (Math.Abs(Pan.X + deltaX) < (Zoom.ScaleX * CanvasWidth - CanvasWidth) / 2)
            Pan.X += deltaX;
        if (Math.Abs(Pan.Y - deltaY) < (Zoom.ScaleY * CanvasHeight - CanvasHeight) / 2)
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
        Countries.Clear();

        _selectedCountries.ForEach(sc =>
        {
            sc.GetPolygons().ToList().ForEach(points =>
            {
                Countries.Add(new CountryViewModel(
                    sc, points.Select(p =>
                        new Point(
                            (p.X + 180) * CanvasWidth / 360,
                            (p.Y + 90) * CanvasHeight / 180 * -1 + CanvasHeight
                        )).ToList()
                ));
            });
        });
    }
}