using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Pandora.Config;
using Pandora.Services;
using Path = System.IO.Path;
using Polygon = NetTopologySuite.Geometries.Polygon;

namespace Pandora.ViewModels;

public partial class EditorViewModel : ViewModelBase
{
    private readonly GeoDataService _geoDataService = new();
    [ObservableProperty] private List<string> _folders = [];
    [ObservableProperty] private FeatureCollection _features = [];

    public EditorViewModel()
    {
        foreach (var fileName in Directory.GetFiles(
                     Path.Combine(
                         AppEnvironment.GetFolderPath(
                             AppEnvironment.FolderEnum.LinuxAppData), "Data", "Maps")))
        {
            Folders.Add(fileName.Split("/")[fileName.Split("/").Length - 1]);
        }
    }

    [RelayCommand]
    private async Task LoadSelectedFile(string fileName)
    {
        try
        {
            var path = Path.Combine(AppEnvironment.GetFolderPath(AppEnvironment.FolderEnum.LinuxAppData), "Data", "Maps",
                fileName);
            Features = await _geoDataService.LoadCollection(path);
            // foreach (var feature in geoJson)
            // {
            //     switch (feature.Geometry)
            //     {
            //         case MultiPolygon multiPolygon:
            //             break;
            //         case Polygon polygon:
            //             break;
            //         default:
            //             throw new ArgumentOutOfRangeException(null);
            //     }
            // }
        }
        catch (Exception e)
        {
            // ignored
        }
    }
}