using System.Drawing;

namespace Bones.WebUI.Leaflet.Models;

/// <summary>
///   A coordinate somewhere on earth, using a Latitude and Longitude
/// </summary>
public class LatLon
{
    /// <summary>
    ///   The latitude
    /// </summary>
    public float Lat { get; set; }

    /// <summary>
    ///   The longitude
    /// </summary>
    public float Lon { get; set; }

    /// <summary>
    ///   The altitude
    /// </summary>
    public float Alt { get; set; }

    /// <summary>
    ///   Converts this LatLon into a PointF
    /// </summary>
    /// <returns></returns>
    public PointF ToPointF() => new(Lat, Lon);

    /// <summary>
    ///   A LatLon that is feeling a little empty inside
    /// </summary>
    public LatLon() { }

    /// <summary>
    ///   A LatLon from a PointF
    /// </summary>
    /// <param name="position"></param>
    public LatLon(PointF position) : this(position.X, position.Y) { }

    /// <summary>
    ///   X, Y
    /// </summary>
    /// <param name="lat"></param>
    /// <param name="lon"></param>
    public LatLon(float lat, float lon)
    {
        Lat = lat;
        Lon = lon;
    }

    /// <summary>
    ///   X, Y, Z
    /// </summary>
    /// <param name="lat"></param>
    /// <param name="lon"></param>
    /// <param name="alt"></param>
    public LatLon(float lat, float lon, float alt) : this(lat, lon)
    {
        Alt = alt;
    }
}