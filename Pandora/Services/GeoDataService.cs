using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Pandora.Models;

namespace Pandora.Services;

public class GeoDataService
{
    private readonly GeoJsonReader _geoJsonReader = new();

    private async Task<FeatureCollection> LoadCountries()
    {
        var assets = AssetLoader.Open(new Uri("avares://Pandora/Data/Core/ne_10m_admin_0_countries.json"));

        using var reader = new StreamReader(assets);
        var json = await reader.ReadToEndAsync();
        return _geoJsonReader.Read<FeatureCollection>(json);
    }

    public async Task<Country?> GetCountry(string name)
    {
        var countries =  await LoadCountries();
        
        var country = countries.FirstOrDefault(f => f.Attributes["NAME"]?.ToString()?.Equals(name, StringComparison.OrdinalIgnoreCase) == true);

        if (country == null) return null;

        return new Country
        {
            Name = country.Attributes["NAME"]?.ToString() ?? "",
            Iso3Code = country.Attributes["ISO_A3"]?.ToString() ?? "",
            Geometry = country.Geometry
        };
    }

    /*     public async Task<FeatureCollection> LoadStatesProvinces()
    {
        var json = await File.ReadAllTextAsync("Data/Maps/states-provinces-10m.json");
        return JsonSerializer.Deserialize<FeatureCollection>(json);
    }
 */
}