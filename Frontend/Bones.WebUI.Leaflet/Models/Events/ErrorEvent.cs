namespace Bones.WebUI.Leaflet.Models.Events;

/// <summary>
///   Uh oh!
/// </summary>
public class ErrorEvent : Event
{
    /// <summary>
    ///   What went wrong
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    ///   Some kind of status code
    /// </summary>
    public int? Code { get; init; }
}