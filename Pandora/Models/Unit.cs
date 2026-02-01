using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;

namespace Pandora.Models;

// Unit of a quiz.
public class Unit(string key, string name, Geometry geometry)
{
    public string Key { get; set; } = key;
    public string Name { get; set; } = name;
    private Geometry Geometry { get; set; } = geometry;

    public IReadOnlyList<List<Point>> GetPolygons()
    {
        var points = new List<List<Point>>();


        // Certain countries are single Polygons as they only encompass single piece of land. Other countries are multi-polygons if they feature - besides the mainlands, some islands or territories overseas.
        switch (Geometry)
        {
            case Polygon polygon:
                // If the country is too small, just draw a circle in its location
                // TODO make it scale dependent
                /*
                const double minArea = 0.05;
                if (Geometry.Area < minArea)
                {
                    var center = Geometry.Centroid;
                    var radius = minArea * 10 * (1 - Geometry.Area);
                    var circlePoints = new List<Point>();
                    const int segments = 20;

                    for (var i = 0; i < segments; i++)
                    {
                        var angle = 2 * Math.PI * i / segments;
                        circlePoints.Add(new Point(
                            center.X + radius * Math.Cos(angle),
                            center.Y + radius * Math.Sin(angle)
                        ));
                    }
                    points.Add(circlePoints);
                    return points;
                }
                */

                // This will add points to the points var that's being passed, hence why it's a void
                var pointList = polygon.ExteriorRing.Coordinates
                    .Select(coord => new Point(coord.X + 180, 180 - (coord.Y + 90))).ToList();
                points.Add(pointList);

                break;
            case MultiPolygon multiPolygon when multiPolygon.Geometries.Length > 0:
            {
                multiPolygon.Geometries
                    .Cast<Polygon>()
                    .OrderByDescending(p => p.Area)
                    .ToList()
                    .ForEach(p =>
                        points.Add(p.ExteriorRing.Coordinates.Select(coord => new Point(coord.X + 180, 180 - (coord.Y + 90)))
                            .ToList()));

                break;
            }
        }

        return points;
    }
}