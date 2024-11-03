using Bones.WebUI.Leaflet.Models.Events;
using Microsoft.JSInterop;

namespace Bones.WebUI.Leaflet.Models;

/// <summary>
///   An interactive layer on the map
/// </summary>
public abstract class InteractiveLayer : Layer
{

    /// <summary>
    /// If false, the layer will not emit mouse events and will act as a part of the underlying map. (events currently not implemented in Bones.BlazorWasmLeaflet)
    /// </summary>
    public bool IsInteractive { get; set; } = true;

    /// <summary>
    /// When true, a mouse event on this layer will trigger the same event on the map (unless L.DomEvent.stopPropagation is used).
    /// </summary>
    public virtual bool IsBubblingMouseEvents { get; set; } = true;

    #region events
    /// <summary>
    ///   Handler for a mouse event
    /// </summary>
    public delegate void MouseEventHandler(InteractiveLayer sender, MouseEvent e);

    /// <summary>
    ///   Handler for a click
    /// </summary>
    public event MouseEventHandler? OnClick;

    /// <summary>
    ///   Event from the JS notifying that the user has clicked
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyClick(MouseEvent eventArgs)
    {
        OnClick?.Invoke(this, eventArgs);
    }

    /// <summary>
    ///   Handler for a double click
    /// </summary>
    public event MouseEventHandler? OnDblClick;

    /// <summary>
    ///   Event from the JS notifying that a double click has taken place
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyDblClick(MouseEvent eventArgs)
    {
        OnDblClick?.Invoke(this, eventArgs);
    }

    /// <summary>
    ///   Handler for the mouse button being pressed
    /// </summary>
    public event MouseEventHandler? OnMouseDown;

    /// <summary>
    ///   Event from the JS notifying that a mouse button has been pressed
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMouseDown(MouseEvent eventArgs)
    {
        OnMouseDown?.Invoke(this, eventArgs);
    }

    /// <summary>
    ///   Handler for the mouse button being released
    /// </summary>
    public event MouseEventHandler? OnMouseUp;

    /// <summary>
    ///   Event from the JS notifying that a mouse button has been released
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMouseUp(MouseEvent eventArgs)
    {
        OnMouseUp?.Invoke(this, eventArgs);
    }

    /// <summary>
    ///   Handler for the mouse entering
    /// </summary>
    public event MouseEventHandler? OnMouseOver;

    /// <summary>
    ///   Event from the JS notifying that the mouse is over this
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMouseOver(MouseEvent eventArgs)
    {
        OnMouseOver?.Invoke(this, eventArgs);
    }

    /// <summary>
    ///   Handler for the mouse leaving
    /// </summary>
    public event MouseEventHandler? OnMouseOut;

    /// <summary>
    ///   Event from the JS notifying that the mouse has been removed
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMouseOut(MouseEvent eventArgs)
    {
        OnMouseOut?.Invoke(this, eventArgs);
    }

    /// <summary>
    ///   Handler for the context menu being opened
    /// </summary>
    public event MouseEventHandler? OnContextMenu;

    /// <summary>
    ///   Event from the JS notifying that the context menu has been opened
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyContextMenu(MouseEvent eventArgs)
    {
        OnContextMenu?.Invoke(this, eventArgs);
    }

    #endregion

}