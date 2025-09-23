using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetTopologySuite.Geometries;
using Pandora.Models;
using Pandora.Services;
using Point = Pandora.Models.Point;

namespace Pandora.ViewModels;

public partial class MapTestViewModel : ViewModelBase
{
    private readonly GeoDataService _geoDataService = new();
    
    [ObservableProperty]
    private Country? _selectedCountry;
    
    [ObservableProperty]
    private string _status = "Ready to load country data...";
    
    [ObservableProperty]
    private bool _isLoading;
    
    // Properties for rendering
    [ObservableProperty]
    private string _countryPathData = "";
    
    [ObservableProperty]
    private double _canvasWidth = 600;
    
    [ObservableProperty]
    private double _canvasHeight = 400;
    
    [RelayCommand]
    private async Task LoadCountry(string countryName)
    {
        IsLoading = true;
        Status = $"Loading {countryName}...";
        CountryPathData = ""; // Clear previous path
        
        try
        {
            SelectedCountry = await _geoDataService.GetCountry(countryName);

            Status = SelectedCountry != null ? $"Loaded {SelectedCountry.Name} ({SelectedCountry.Iso3Code})" : $"Country '{countryName}' not found";
            if (SelectedCountry != null)
            {
                Status = $"Loaded {SelectedCountry.Name} ({SelectedCountry.Iso3Code})";
                
                // Calculate scaling to fit the canvas
                var bounds = SelectedCountry.GetBounds();
                var width = bounds.MaxX - bounds.MinX;
                var height = bounds.MaxY - bounds.MinY;
                
                // Scale to fit canvas with some padding
                const int padding = 50;
                var scaleX = (CanvasWidth - padding * 2) / width;
                var scaleY = (CanvasHeight - padding * 2) / height;
                var scale = Math.Min(scaleX, scaleY); // Keep aspect ratio
                
                // Center the country
                var offsetX = (CanvasWidth - width * scale) / 2 - bounds.MinX * scale;
                var offsetY = (CanvasHeight - height * scale) / 2 - bounds.MinY * scale;
                
                // Generate path data
                CountryPathData = SelectedCountry.ToPathGeometryString(scale, scale, offsetX, offsetY);
                
                Console.WriteLine($"Generated path data: {CountryPathData[..Math.Min(100, CountryPathData.Length)]}...");
            }
            else
            {
                Status = $"Country '{countryName}' not found";
            }
        }
        catch (Exception ex)
        {
            Status = $"Error loading {countryName}: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}