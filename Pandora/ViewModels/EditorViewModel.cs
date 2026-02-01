using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetTopologySuite.Features;
using Pandora.Config;
using Pandora.Models;
using Pandora.Services;
using Path = System.IO.Path;

namespace Pandora.ViewModels;

public partial class EditorViewModel : ViewModelBase
{
    // UI
    [ObservableProperty] private int _canvasWidth = 1200;
    [ObservableProperty] private int _canvasHeight = 600;
    [ObservableProperty] private TranslateTransform _canvasPan = new(0.0, 0.0);
    [ObservableProperty] private ScaleTransform _canvasZoom = new(1.0, 1.0);
    [ObservableProperty] private Avalonia.Point _cursorLocation;

    // Data
    private readonly GeoDataService _geoDataService = new();
    private FeatureCollection _geoJson = [];
    [ObservableProperty] private List<string> _folders = [];
    [ObservableProperty] private string? _selectedFolder;
    [ObservableProperty] private List<string> _attributes = [];
    [ObservableProperty] private string? _selectedAttribute;

    [ObservableProperty] private ObservableCollection<UnitViewModel> _units = [];

    public EditorViewModel()
    {
        foreach (var fileName in AppEnvironment.GetFiles(
                     Path.Combine(AppEnvironment.ApplicationPath, "Data", "Maps")))
        {
            Folders.Add(fileName);
        }
    }

    [RelayCommand]
    private async Task LoadSelectedFile()
    {
        if (SelectedFolder != null && SelectedFolder.Equals(string.Empty)) return;
        try
        {
            var path = Path.Combine(AppEnvironment.ApplicationPath, "Data",
                "Maps",
                SelectedFolder!);
            _geoJson = await _geoDataService.LoadCollection(path);
            Attributes = _geoJson.SelectMany(feature => feature.Attributes.GetNames().OrderBy(v => v)).Distinct()
                .ToList();
            SelectedAttribute = Attributes[0];
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    [RelayCommand]
    private void LoadUnits()
    {
        Units.Clear();
        if (_geoJson.Count < 1) return;
        var units = _geoJson.Select(feature => new Unit
        (
            (string)feature.Attributes[SelectedAttribute],
            (string)feature.Attributes[SelectedAttribute],
            feature.Geometry
        )).ToList();
        // TODO scale x and y
        var maxX = units.Max(unit =>
            unit.GetPolygons().ToList().Max(points => points.ToList().Max(point => point.X))
        );
        var maxY = units.Max(unit =>
            unit.GetPolygons().ToList().Max(points => points.ToList().Max(point => point.Y))
        );
        var minX = units.Min(unit =>
            unit.GetPolygons().ToList().Min(points => points.ToList().Min(point => point.X))
        );
        var minY = units.Min(unit =>
            unit.GetPolygons().ToList().Min(points => points.ToList().Min(point => point.Y))
        );

        var width = maxX + (maxX - (maxX - minX));
        var height = maxY + (maxY - (maxY - minY));

        var scale = Math.Min(CanvasWidth, CanvasHeight) / Math.Min((maxX-minX),(maxY-minY)) / 1.3;
        units.ForEach(unit =>
        {
            unit.GetPolygons().ToList().ForEach(points =>
            {
                Units.Add(new UnitViewModel(
                    unit, points.Select(p => new Avalonia.Point(p.X, p.Y)).ToList()));
            });
        });
        CanvasZoom = new ScaleTransform(scale, scale);
        CanvasPan = new TranslateTransform((CanvasWidth / 2 - width / 2) * scale,
            (CanvasHeight / 2 - height / 2) * scale);
    }
}