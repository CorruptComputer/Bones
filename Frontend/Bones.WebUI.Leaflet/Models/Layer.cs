using Bones.WebUI.Leaflet.Models.Events;
using Microsoft.JSInterop;

namespace Bones.WebUI.Leaflet.Models;

/// <summary>
///   A layer for the map
/// </summary>
public abstract class Layer
{
    /// <summary>
    /// Unique identifier used by the interoperability service on the client side to identify layers.
    /// </summary>
    public string Id { get; } = Guid.NewGuid().ToString();

    /// <summary>
    /// By default, the layer will be added to the map's overlay pane. Overriding this option will cause the layer to be placed on another pane by default.
    /// </summary>
    public virtual string Pane { get; set; } = "overlayPane";

    /// <summary>
    /// String to be shown in the attribution control, e.g. "© OpenStreetMap contributors". It describes the layer data and is often a legal obligation towards copyright holders and tile providers.
    /// </summary>
    public string? Attribution { get; set; }

    /// <summary>
    /// The tooltip assigned to this marker.
    /// </summary>
    public Tooltip? Tooltip { get; set; }

    /// <summary>
    /// The popup shown when the marker is clicked.
    /// </summary>
    public Popup? Popup { get; set; }

    #region events

    /// <summary>
    ///   The handler for an event
    /// </summary>
    public delegate Task EventHandler(Layer sender, Event e);

    /// <summary>
    ///   The handler for an event
    /// </summary>
    public event EventHandler? OnAdd;

    /// <summary>
    ///   Event from the JS notifying of an add
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyAdd(Event eventArgs)
    {
        OnAdd?.Invoke(this, eventArgs);
    }

    /// <summary>
    ///   The handler for a remove
    /// </summary>
    public event EventHandler? OnRemove;

    /// <summary>
    ///   Event from the JS notifying of a remove
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyRemove(Event eventArgs)
    {
        OnRemove?.Invoke(this, eventArgs);
    }

    /// <summary>
    ///   The handler for a popup
    /// </summary>
    public delegate Task PopupEventHandler(Layer sender, PopupEvent e);

    /// <summary>
    ///   The handler for a popup open
    /// </summary>
    public event PopupEventHandler? OnPopupOpen;

    /// <summary>
    ///   Event from the JS notifying of a popup open
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyPopupOpen(PopupEvent eventArgs)
    {
        OnPopupOpen?.Invoke(this, eventArgs);
    }
    /// <summary>
    ///   The handler for a popup close
    /// </summary>

    public event PopupEventHandler? OnPopupClose;

    /// <summary>
    ///   Event from the JS notifying of a popup close
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyPopupClose(PopupEvent eventArgs)
    {
        OnPopupClose?.Invoke(this, eventArgs);
    }

    /// <summary>
    ///   The handler for a tooltip
    /// </summary>
    public delegate Task TooltipEventHandler(Layer sender, TooltipEvent e);

    /// <summary>
    ///   The handler for a tooltip open
    /// </summary>
    public event TooltipEventHandler? OnTooltipOpen;

    /// <summary>
    ///   Event from the JS notifying of a tooltip open
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyTooltipOpen(TooltipEvent eventArgs)
    {
        OnTooltipOpen?.Invoke(this, eventArgs);
    }

    /// <summary>
    ///   The handler for a tooltip close
    /// </summary>
    public event TooltipEventHandler? OnTooltipClose;

    /// <summary>
    ///   Event from the JS notifying of a tooltip close
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyTooltipClose(TooltipEvent eventArgs)
    {
        OnTooltipClose?.Invoke(this, eventArgs);
    }

    #endregion
}