using Chapmans.Peak.Common;

namespace Chapmans.Peak.Route;

public interface IRouteProvider
{
    /// <summary>
    /// Returns a collection of points
    /// </summary>
    IEnumerable<Point> GetRoute();
}