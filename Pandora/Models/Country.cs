using System;
using System.Linq;
using System.Text;
using NetTopologySuite.Geometries;

namespace Pandora.Models;

public class Country
{
    public string Name { get; set; } = "";
    public string Iso3Code { get; set; } = "";
    public Geometry Geometry { get; init; } = null!;
    public double[]? BoundingBox { get; set; }
    
    // Helper method to get simple coordinate array for rendering
    public Point[][] GetPolygonCoordinates()
    {
        return Geometry switch
        {
            Polygon polygon => [polygon.ExteriorRing.Coordinates.Select(c => new Point(c.X, c.Y)).ToArray()],
            MultiPolygon multiPolygon => multiPolygon.Geometries.Cast<Polygon>()
                .Select(p => p.ExteriorRing.Coordinates.Select(c => new Point(c.X, c.Y)).ToArray())
                .ToArray(),
            _ => []
        };
    }
    
    // Get bounding box for centering/scaling
    public (double MinX, double MinY, double MaxX, double MaxY) GetBounds()
    {
        var envelope = Geometry.EnvelopeInternal;
        return (envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY);
    }
    
    // Convert to Avalonia PathGeometry string
    public string ToPathGeometryString(double scaleX = 1.0, double scaleY = 1.0, double offsetX = 0, double offsetY = 0)
    {
        var pathBuilder = new StringBuilder();
        
        if (Geometry is Polygon polygon)
        {
            AppendPolygonToPath(polygon, pathBuilder, scaleX, scaleY, offsetX, offsetY);
        }
        else if (Geometry is MultiPolygon multiPolygon)
        {
            foreach (var geometry in multiPolygon.Geometries)
            {
                var poly = (Polygon)geometry;
                AppendPolygonToPath(poly, pathBuilder, scaleX, scaleY, offsetX, offsetY);
            }
        }
        
        return pathBuilder.ToString();
    }
    
    private void AppendPolygonToPath(Polygon polygon, StringBuilder pathBuilder, double scaleX, double scaleY, double offsetX, double offsetY)
    {
        var coordinates = polygon.ExteriorRing.Coordinates;
        if (coordinates.Length == 0) return;
        
        // Move to first point
        var firstPoint = coordinates[0];
        pathBuilder.Append($"M {(firstPoint.X * scaleX + offsetX):F2},{(-firstPoint.Y * scaleY + offsetY):F2} ");
        
        // Line to subsequent points
        for (var i = 1; i < coordinates.Length; i++)
        {
            var point = coordinates[i];
            pathBuilder.Append($"L {(point.X * scaleX + offsetX):F2},{(-point.Y * scaleY + offsetY):F2} ");
        }
        
        // Close the path
        pathBuilder.Append("Z ");
    }
}

public class Point(double x, double y)
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
}