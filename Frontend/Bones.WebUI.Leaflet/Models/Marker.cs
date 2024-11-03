using System.Drawing;
using Bones.WebUI.Leaflet.Models.Events;
using Microsoft.JSInterop;

namespace Bones.WebUI.Leaflet.Models;

/// <summary>
///   A marker on the map
/// </summary>
public class Marker : InteractiveLayer
{
    /// <summary>
    /// The position of the marker on the map.
    /// </summary>
    public LatLon Position { get; set; }

    /// <summary>
    /// Icon instance to use for rendering the marker. See <see href="https://leafletjs.com/reference-1.5.0.html#icon">Icon documentation</see> for details on how to customize the marker icon. If not specified, a common instance of <see href="https://leafletjs.com/reference-1.5.0.html#icon-default">L.Icon.Default</see> is used.
    /// </summary>
    public Icon? Icon { get; set; }

    /// <summary>
    /// Whether the marker can be tabbed to with a keyboard and clicked by pressing enter.
    /// </summary>
    public bool IsKeyboardAccessible { get; set; } = true;

    /// <summary>
    /// Text for the browser tooltip that appear on marker hover (no tooltip by default).
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Text for the alt attribute of the icon image (useful for accessibility).
    /// </summary>
    public string Alt { get; set; } = string.Empty;

    /// <summary>
    /// By default, marker images zIndex is set automatically based on its latitude. Use this option if you want to put the marker on top of all others (or below), specifying a high value like 1000 (or high negative value, respectively).
    /// </summary>
    public int ZIndexOffset { get; set; }

    /// <summary>
    /// The opacity of the marker.
    /// </summary>
    public double Opacity { get; set; } = 1.0;

    /// <summary>
    /// If true, the marker will get on top of others when you hover the mouse over it.
    /// </summary>
    public bool RiseOnHover { get; set; }

    /// <summary>
    /// The z-index offset used for the riseOnHover feature.
    /// </summary>
    public int RiseOffset { get; set; } = 250;

    /// <inheritdoc />
    public override string Pane { get; set; } = "markerPane";

    /// <inheritdoc />
    public override bool IsBubblingMouseEvents { get; set; } = false;

    /// <summary>
    /// Whether the marker is draggable with mouse/touch or not.
    /// </summary>
    public bool Draggable { get; set; }

    /// <summary>
    /// Whether to pan the map when dragging this marker near its edge or not.
    /// </summary>
    public bool UseAutoPan { get; set; }

    /// <summary>
    /// Distance (in pixels to the left/right and to the top/bottom) of the map edge to start panning the map.
    /// </summary>
    public Point AutoPanPadding { get; set; } = new(50, 50);

    /// <summary>
    /// Number of pixels the map should pan by.
    /// </summary>
    public int AutoPanSpeed { get; set; } = 10;

    /// <summary>
    ///   Marker at a coordinate
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Marker(float x, float y) : this(new LatLon(x, y)) { }

    /// <summary>
    ///   Marker at a point
    /// </summary>
    /// <param name="position"></param>
    public Marker(PointF position) : this(position.X, position.Y) { }

    /// <summary>
    ///   Marker at a lat/lon
    /// </summary>
    /// <param name="latLon"></param>
    public Marker(LatLon latLon)
    {
        Position = latLon;
    }

    #region events

    /// <summary>
    ///   The handler for a drag event
    /// </summary>
    public delegate void DragEventHandler(Marker sender, DragEvent e);

    /// <summary>
    ///   The handler for a drag event
    /// </summary>
    public event DragEventHandler? OnMove;

    /// <summary>
    ///   Event from the JS notifying of movement
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMove(DragEvent eventArgs)
    {
        OnMove?.Invoke(this, eventArgs);
    }

    /// <summary>
    ///   The handler for a drag start event
    /// </summary>
    public delegate void EventHandlerMarker(Marker sender, Event e);

    /// <summary>
    ///   The handler for a drag start event
    /// </summary>
    public event EventHandlerMarker? OnDragStart;

    /// <summary>
    ///   Event from the JS notifying of the start of a drag
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyDragStart(Event eventArgs)
    {
        OnDragStart?.Invoke(this, eventArgs);
    }

    /// <summary>
    ///   The handler for a move start event
    /// </summary>
    public event EventHandlerMarker? OnMoveStart;

    /// <summary>
    ///   Event from the JS notifying of the start of a move
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMoveStart(Event eventArgs)
    {
        OnMoveStart?.Invoke(this, eventArgs);
    }

    /// <summary>
    ///   The handler for a drag event
    /// </summary>
    public event DragEventHandler? OnDrag;

    /// <summary>
    ///   Event from the JS notifying of dragging
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyDrag(DragEvent eventArgs)
    {
        OnDrag?.Invoke(this, eventArgs);
    }

    /// <summary>
    ///   The handler for a drag end event
    /// </summary>
    public delegate void DragEndEventHandler(Marker sender, DragEndEvent e);

    /// <summary>
    ///   The handler for a drag end event
    /// </summary>
    public event DragEndEventHandler? OnDragEnd;

    /// <summary>
    ///   Event from the JS notifying the end of a drag
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyDragEnd(DragEndEvent eventArgs)
    {
        OnDragEnd?.Invoke(this, eventArgs);
    }

    /// <summary>
    ///   The handler for a move end event
    /// </summary>
    public event EventHandlerMarker? OnMoveEnd;

    /// <summary>
    ///   Event from the JS notifying the end of a move
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMoveEnd(Event eventArgs)
    {
        OnMoveEnd?.Invoke(this, eventArgs);
    }

    #endregion

}