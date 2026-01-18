using System.IO;
using System.Threading.Tasks;
using NetTopologySuite.Features;
using NetTopologySuite.IO;

namespace Pandora.Services;

public class GeoDataService
{
    private readonly GeoJsonReader _geoJsonReader = new();

    public async Task<FeatureCollection> LoadCollection(string path)
    {
        using StreamReader reader = new(path);
        var content = await reader.ReadToEndAsync();
        return _geoJsonReader.Read<FeatureCollection>(content);
    }
}