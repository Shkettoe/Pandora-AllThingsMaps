using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NetTopologySuite.Geometries;
using Point = Avalonia.Point;

namespace Pandora.Models;

public class Country
{
    public string Name { get; set; } = "";
    public string Iso3Code { get; set; } = "";
    public Geometry Geometry { get; init; } = null!;

    public IReadOnlyList<List<Point>> GetPolygons()
    {
        var points = new List<List<Point>>();

        // Certain countries are single Polygons as they only encompass single piece of land. Other countries are multi-polygons if they feature - besides the mainlands, some islands or territories overseas.
        switch (Geometry)
        {
            case Polygon polygon:
                // This will add points to the points var that's being passed, hence why it's a void
                var pointList = polygon.ExteriorRing.Coordinates.Select(coord => new Point(coord.X, coord.Y)).ToList();
                points.Add(pointList);

                break;
            case MultiPolygon multiPolygon when multiPolygon.Geometries.Length > 0:
            {
                // TODO: check dynamically for other polygons that are big enough and close enough to be significant and render them too. E.g. - Sicily for Italy. False positive e.g. - Greenland for Denmark (pointless to render) 
                multiPolygon.Geometries
                    .Cast<Polygon>()
                    .OrderByDescending(p => p.Area)
                    .ToList()
                    .ForEach(p =>
                        points.Add(p.ExteriorRing.Coordinates.Select(coord => new Point(coord.X, coord.Y)).ToList()));
                //
                // foreach (var coord in largestPolygon.ExteriorRing.Coordinates)
                // {
                //     points.Add(new Point(coord.X, coord.Y));
                // }

                break;
            }
        }

        return points;
    }

    // Get simple coordinate points for Polygon binding
    public IReadOnlyList<Point> GetPolygonPoints()
    {
        var points = new ObservableCollection<Point>();

        // Certain countries are single Polygons as they only encompass single piece of land. Other countries are multi-polygons if they feature - besides the mainlands, some islands or territories overseas.
        switch (Geometry)
        {
            case Polygon polygon:
                // This will add points to the points var that's being passed, hence why it's a void
                foreach (var coord in polygon.ExteriorRing.Coordinates)
                {
                    points.Add(new Point(coord.X, coord.Y));
                }

                break;
            case MultiPolygon multiPolygon when multiPolygon.Geometries.Length > 0:
            {
                // TODO: check dynamically for other polygons that are big enough and close enough to be significant and render them too. E.g. - Sicily for Italy. False positive e.g. - Greenland for Denmark (pointless to render) 
                var largestPolygon = multiPolygon.Geometries
                    .Cast<Polygon>()
                    .OrderByDescending(p => p.Area)
                    .FirstOrDefault();

                foreach (var coord in largestPolygon!.ExteriorRing.Coordinates)
                {
                    points.Add(new Point(coord.X, coord.Y));
                }

                break;
            }
        }

        return points;
    }

    /**
     * @deprecated
     */
    private static void AddPolygonPoints(Polygon polygon, ObservableCollection<Point> points,
        double canvasWidth, double canvasHeight)
    {
        var coordinates = polygon.ExteriorRing.Coordinates;
        if (coordinates.Length == 0) return;

        // Calculate bounds for scaling - perhaps you don't want to do this here
        var minX = coordinates.Min(c => c.X);
        var maxX = coordinates.Max(c => c.X);
        var minY = coordinates.Min(c => c.Y);
        var maxY = coordinates.Max(c => c.Y);

        var width = maxX - minX;
        var height = maxY - minY;

        // Calculate scale to fit canvas with padding
        const int padding = 50;
        var scaleX = (canvasWidth - padding * 2) / width;
        var scaleY = (canvasHeight - padding * 2) / height;
        var scale = Math.Min(scaleX, scaleY); // Keep aspect ratio

        // Calculate offset to center the polygon
        var offsetX = (canvasWidth - width * scale) / 2 - minX * scale;
        var offsetY = (canvasHeight - height * scale) / 2 - minY * scale;

        // Convert coordinates to Avalonia Points
        foreach (var coord in coordinates)
        {
            var x = coord.X * scale + offsetX;
            var y = canvasHeight - (coord.Y * scale + offsetY); // Flip Y axis for screen coordinates
            points.Add(new Point(x, y));
        }
    }
}