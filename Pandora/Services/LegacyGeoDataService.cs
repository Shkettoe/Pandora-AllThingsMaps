using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Pandora.Models;

namespace Pandora.Services;

public class LegacyGeoDataService
{
    private readonly GeoJsonReader _geoJsonReader = new();

    private async Task<FeatureCollection> LoadCountries()
    {
        var assets = AssetLoader.Open(new Uri("avares://Pandora/Data/Core/ne_50m_admin_0_countries.json"));

        using var reader = new StreamReader(assets);
        var json = await reader.ReadToEndAsync();
        return _geoJsonReader.Read<FeatureCollection>(json);
    }

    public async Task<Country?> GetCountry(string name)
    {
        var countries = await LoadCountries();

        var country = countries.FirstOrDefault(f =>
            f.Attributes["NAME"]?.ToString()?.Equals(name, StringComparison.OrdinalIgnoreCase) == true);

        if (country == null) return null;

        return new Country
        {
            Name = country.Attributes["NAME"]?.ToString() ?? "",
            Iso3Code = country.Attributes["ISO_A3"]?.ToString() ?? "",
            Geometry = country.Geometry
        };
    }

    public async Task<IEnumerable<Country>?> GetCountriesByContinent(string continent)
    {
        var countries = await LoadCountries();

        IEnumerable<IFeature> europeanCountries = countries.Where(c =>
            c.Attributes["CONTINENT"]?.ToString()?.Equals(continent, StringComparison.OrdinalIgnoreCase) == true);

        List<Country> countriesList = [];

        europeanCountries.ToList().ForEach(c =>
        {
            countriesList.Add(new Country
            {
                Name = c.Attributes["NAME"]?.ToString() ?? "",
                Iso3Code = c.Attributes["ISO_A3"]?.ToString() ?? "",
                Geometry = c.Geometry
            });
        });

        return countriesList;
    }

    public async Task<IEnumerable<Country>> GetAllCountries()
    {
        var countries = await LoadCountries();

        List<Country> countriesList = [];

        countries.OrderByDescending(c => c.Geometry.Area).ToList().ForEach(c => countriesList.Add(new Country
        {
            Name = c.Attributes["NAME"]?.ToString() ?? "",
            Iso3Code = c.Attributes["ISO_A3"]?.ToString() ?? "",
            Geometry = c.Geometry
        }));

        return countriesList;
    }

    /*     public async Task<FeatureCollection> LoadStatesProvinces()
    {
        var json = await File.ReadAllTextAsync("Data/Maps/states-provinces-10m.json");
        return JsonSerializer.Deserialize<FeatureCollection>(json);
    }
 */
}