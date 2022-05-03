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
    private readonly string _kmlFilePath;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="kmlFilePath"></param>
    public KmlFileRouteProvider(string kmlFilePath)
    {
        Guard.Against.NullOrEmpty(kmlFilePath, nameof(kmlFilePath));
        _kmlFilePath = kmlFilePath;
    }

    /// <summary>
    /// Get the route for the provided folder name
    /// </summary>
    /// <param name="folderName"></param>
    /// <returns>The points contained within the line</returns>
    private IEnumerable<Point> GetRoute(string folderName)
    {
        Guard.Against.NullOrEmpty(folderName);
        if (!File.Exists(_kmlFilePath))
        {
            throw new FileNotFoundException($"Unable to find KML file : {_kmlFilePath}");
        }

        var fileStream = File.OpenText(_kmlFilePath);
        var kmlFile = KmlFile.Load(fileStream);

        var kmlObject = kmlFile.FindObject(folderName);
        if (kmlObject == null)
        {
            throw new InvalidOperationException($"Unable to find object with the id : {folderName}");
        }

        // Get the line string
        foreach (var lineString in kmlObject.Flatten().OfType<LineString>())
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