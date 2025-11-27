using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetTopologySuite.Geometries;
using Pandora.Models;
using Pandora.Services;

namespace Pandora.ViewModels;

public partial class MapTestViewModel : ViewModelBase
{
    private readonly GeoDataService _geoDataService = new();

    [ObservableProperty]
    private Country? _selectedCountry;

    [ObservableProperty]
    private bool _isLoading;

    // Properties for rendering
    [ObservableProperty]
    private ObservableCollection<Avalonia.Point> _countryPoints = [];
    
    [ObservableProperty]
    private double _canvasWidth = 600;

    [ObservableProperty]
    private double _canvasHeight = 400;

    [RelayCommand]
    private async Task LoadCountry(string countryName)
    {
        IsLoading = true;
        CountryPoints.Clear(); // Clear previous points

        try
        {
            SelectedCountry = await _geoDataService.GetCountry(countryName);

            if (SelectedCountry != null)
            {
                // Get polygon points scaled to canvas
                var points = SelectedCountry.GetPolygonPoints(CanvasWidth, CanvasHeight);

                // Replace the collection contents
                CountryPoints.Clear();
                foreach (var point in points)
                {
                    CountryPoints.Add(point);
                }

                Console.WriteLine($"Generated {CountryPoints.Count} points for {SelectedCountry.Name}");
                if (CountryPoints.Count > 0)
                {
                    Console.WriteLine($"First point: {CountryPoints[0]}, Last point: {CountryPoints[^1]}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}