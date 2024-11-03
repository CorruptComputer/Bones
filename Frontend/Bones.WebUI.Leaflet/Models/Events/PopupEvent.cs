namespace Bones.WebUI.Leaflet.Models.Events;

/// <summary>
///   An event that should cause a popup to appear
/// </summary>
public class PopupEvent : Event
{
    /// <summary>
    ///   The popup that should be shown
    /// </summary>
    public Popup? Popup { get; init; }
}