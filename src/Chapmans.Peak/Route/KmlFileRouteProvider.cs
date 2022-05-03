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
    private readonly string _folderName;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="kmlFilePath">The path of the KML file</param>
    /// <param name="folderName">Name of the folder within the KML file that contains the line points</param>
    public KmlFileRouteProvider(string kmlFilePath, string folderName)
    {
        Guard.Against.NullOrEmpty(kmlFilePath, nameof(kmlFilePath));
        Guard.Against.NullOrEmpty(folderName, nameof(folderName));
        
        _kmlFilePath = kmlFilePath;
        _folderName = folderName;
    }

    /// <inheritdoc />
    public IEnumerable<Point> GetRoute()
    {
        if (!File.Exists(_kmlFilePath))
        {
            throw new FileNotFoundException($"Unable to find KML file : {_kmlFilePath}");
        }

        var fileStream = File.OpenText(_kmlFilePath);
        var kmlFile = KmlFile.Load(fileStream);

        var kmlObject = kmlFile.FindObject(_folderName);
        if (kmlObject == null)
        {
            throw new InvalidOperationException($"Unable to find object with the id : {_folderName}");
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