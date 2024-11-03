namespace Bones.WebUI.Leaflet.Models.Events;

/// <summary>
///   Something happened
/// </summary>
public class Event
{
    /// <summary>
    ///   The type of thing that happened
    /// </summary>
    public string? Type { get; init; }
}