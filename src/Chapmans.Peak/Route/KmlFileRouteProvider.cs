using Ardalis.GuardClauses;
using SharpKml.Dom;
using SharpKml.Engine;
using Point = Chapmans.Peak.Common.Point;

namespace Chapmans.Peak.Route;

/// <summary>
/// Provides the ability to read a route from a KML file
/// </summary>
public class KmlFileRouteProvider : IRouteProvider
{
    private readonly string _embeddedKmlFile;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="embeddedKmlFile"></param>
    public KmlFileRouteProvider(string embeddedKmlFile)
    {
        Guard.Against.NullOrEmpty(embeddedKmlFile, nameof(embeddedKmlFile));
        _embeddedKmlFile =  Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "Files",
            embeddedKmlFile);
    }

    /// <summary>
    /// Get the route for the provided folder name
    /// </summary>
    /// <returns>The points contained within the line</returns>
    public IEnumerable<Point> GetRoute()
    {
        if (!File.Exists(_embeddedKmlFile))
        {
            throw new FileNotFoundException($"Unable to find KML file : {_embeddedKmlFile}");
        }

        var fileStream = File.OpenText(_embeddedKmlFile);
        var kmlFile = KmlFile.Load(fileStream);

        // Get the line string
        foreach (var lineString in kmlFile.Root.Flatten().OfType<LineString>())
        {
            return lineString.Coordinates.Select(x => new Point
            {
                Latitude = x.Latitude,
                Longitude = x.Longitude
            });
        }

        // Prefer empty collections over null
        return Array.Empty<Point>();
    }
}