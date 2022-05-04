using Chapmans.Peak.Common;

namespace Chapmans.Peak.Route;

/// <summary>
/// Interface to define basic requirement for a route provider 
/// </summary>
public interface IRouteProvider
{
    /// <summary>
    /// Get a route
    /// </summary>
    /// <returns></returns>
    IEnumerable<Point> GetRoute();
}