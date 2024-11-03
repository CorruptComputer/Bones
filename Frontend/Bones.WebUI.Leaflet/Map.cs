using System.Collections;
using Bones.WebUI.Leaflet.Models;
using System.Collections.ObjectModel;
using System.Drawing;
using Microsoft.JSInterop;
using Bones.WebUI.Leaflet.Models.Events;
using System.Collections.Specialized;
using Bones.WebUI.Leaflet.Exceptions;
using Microsoft.AspNetCore.Components.Web;

namespace Bones.WebUI.Leaflet;

/// <summary>
///   The map
/// </summary>
public class Map
{
    /// <summary>
    /// Initial geographic center of the map
    /// </summary>
    public LatLon Center { get; set; } = new();

    /// <summary>
    /// Initial map zoom level
    /// </summary>
    public float Zoom { get; set; }

    /// <summary>
    /// Minimum zoom level of the map. If not specified and at least one 
    /// GridLayer or TileLayer is in the map, the lowest of their minZoom
    /// options will be used instead.
    /// </summary>
    public float? MinZoom { get; set; }

    /// <summary>
    /// Maximum zoom level of the map. If not specified and at least one
    /// GridLayer or TileLayer is in the map, the highest of their maxZoom
    /// options will be used instead.
    /// </summary>
    public float? MaxZoom { get; set; }

    /// <summary>
    /// When this option is set, the map restricts the view to the given
    /// geographical bounds, bouncing the user back if the user tries to pan
    /// outside the view.
    /// </summary>
    public Tuple<LatLon, LatLon>? MaxBounds { get; set; }

    /// <summary>
    /// Whether a zoom control is added to the map by default.
    /// <para/>
    /// Defaults to true.
    /// </summary>
    public bool ZoomControl { get; set; } = true;

    /// <summary>
    /// Event raised when the component has finished its first render.
    /// </summary>
    public event Action? OnInitialized;

    /// <summary>
    ///   The ID for this map
    /// </summary>
    public string Id { get; }

    private readonly ObservableCollection<Layer> _layers = new();

    private readonly IJSRuntime _jsRuntime;

    private bool _isInitialized;

    /// <summary>
    ///   Creates the map 
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Map(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        Id = Guid.NewGuid().ToString();

        _layers.CollectionChanged += OnLayersChanged;
    }

    /// <summary>
    /// This method MUST be called only once by the Blazor component upon rendering, and never by the user.
    /// </summary>
    public void RaiseOnInitialized()
    {
        _isInitialized = true;
        OnInitialized?.Invoke();
    }

    /// <summary>
    /// Add a layer to the map.
    /// </summary>
    /// <param name="layer">The layer to be added.</param>
    /// <exception cref="System.ArgumentNullException">Throws when the layer is null.</exception>
    /// <exception cref="UninitializedMapException">Throws when the map has not been yet initialized.</exception>
    public void AddLayer(Layer layer)
    {
        ArgumentNullException.ThrowIfNull(layer);

        if (!_isInitialized)
        {
            throw new UninitializedMapException();
        }

        _layers.Add(layer);
    }

    /// <summary>
    /// Remove a layer from the map.
    /// </summary>
    /// <param name="layer">The layer to be removed.</param>
    /// <exception cref="System.ArgumentNullException">Throws when the layer is null.</exception>
    /// <exception cref="UninitializedMapException">Throws when the map has not been yet initialized.</exception>
    public void RemoveLayer(Layer layer)
    {
        ArgumentNullException.ThrowIfNull(layer);

        if (!_isInitialized)
        {
            throw new UninitializedMapException();
        }

        _layers.Remove(layer);
    }

    /// <summary>
    /// Get a read only collection of the current layers.
    /// </summary>
    /// <returns>A read only collection of layers.</returns>
    public IReadOnlyCollection<Layer> GetLayers()
    {
        return _layers.ToList().AsReadOnly();
    }

    private void OnLayersChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args)
        {
            case { Action: NotifyCollectionChangedAction.Add, NewItems: not null }:
                {
                    Task addTask = AddLayersAsync(args.NewItems);
                    break;
                }
            case { Action: NotifyCollectionChangedAction.Remove, OldItems: not null }:
                {
                    Task removeTask = RemoveLayersAsync(args.OldItems);
                    break;
                }
            case { Action: NotifyCollectionChangedAction.Replace or NotifyCollectionChangedAction.Move, OldItems: not null, NewItems: not null }:
                {
                    Task removeTask = RemoveLayersAsync(args.OldItems);
                    Task addTask = AddLayersAsync(args.NewItems);
                    break;
                }
        }
    }

    private async Task AddLayersAsync(IList newItemsParam)
    {
        foreach (object? newItem in newItemsParam)
        {
            if (newItem is Layer layer)
            {
                await LeafletInterops.AddLayerAsync(_jsRuntime, Id, layer);
                Console.WriteLine($"Layer added: {layer.Id}");
            }
        }
    }

    private async Task RemoveLayersAsync(IList oldItemsParam)
    {
        foreach (object? oldItem in oldItemsParam)
        {
            if (oldItem is Layer layer)
            {
                await LeafletInterops.RemoveLayerAsync(_jsRuntime, Id, layer.Id);
                Console.WriteLine($"Layer removed: {layer.Id}");
            }
        }
    }

    /// <summary>
    ///   Fits the map to the given bounds
    /// </summary>
    /// <param name="corner1"></param>
    /// <param name="corner2"></param>
    /// <param name="padding"></param>
    /// <param name="maxZoom"></param>
    public async Task FitBoundsAsync(PointF corner1, PointF corner2, PointF? padding = null, float? maxZoom = null)
    {
        await LeafletInterops.FitBoundsAsync(_jsRuntime, Id, corner1, corner2, padding, maxZoom);
    }

    /// <summary>
    ///   Pans the map to the given coordinates
    /// </summary>
    /// <param name="position"></param>
    /// <param name="animate"></param>
    /// <param name="duration"></param>
    /// <param name="easeLinearity"></param>
    /// <param name="noMoveStart"></param>
    public async Task PanToAsync(PointF position, bool animate = false, float duration = 0.25f, float easeLinearity = 0.25f, bool noMoveStart = false)
    {
        await LeafletInterops.PanToAsync(_jsRuntime, Id, position, animate, duration, easeLinearity, noMoveStart);
    }

    /// <summary>
    ///   Gets the centerpoint of the current map view
    /// </summary>
    /// <returns></returns>
    public async Task<LatLon> GetCenterAsync()
    {
        return await LeafletInterops.GetCenterAsync(_jsRuntime, Id);
    }

    /// <summary>
    ///   Gets the current zoom of the map
    /// </summary>
    /// <returns></returns>
    public async Task<float> GetZoomAsync()
    {
        return await LeafletInterops.GetZoomAsync(_jsRuntime, Id);
    }

    /// <summary>
    /// Increases the zoom level by one notch.
    /// 
    /// If <c>shift</c> is held down, increases it by three.
    /// </summary>
    public async Task ZoomInAsync(MouseEventArgs e) => await LeafletInterops.ZoomInAsync(_jsRuntime, Id, e);

    /// <summary>
    /// Decreases the zoom level by one notch.
    /// 
    /// If <c>shift</c> is held down, decreases it by three.
    /// </summary>
    public async Task ZoomOutAsync(MouseEventArgs e) => await LeafletInterops.ZoomOutAsync(_jsRuntime, Id, e);

    #region events
    /// <summary>
    ///   The event handler
    /// </summary>
    public delegate Task MapEventHandler(object sender, Event e);

    /// <summary>
    ///   The event for the map being unloaded
    /// </summary>
    public event MapEventHandler? OnUnload;

    /// <summary>
    ///   Event from the JS notifying of the unload
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyUnload(Event e) => OnUnload?.Invoke(this, e);

    /// <summary>
    ///   The event for the map being reset
    /// </summary>
    public event MapEventHandler? OnViewReset;

    /// <summary>
    ///   Event from the JS notifying the view has been reset
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyViewReset(Event e) => OnViewReset?.Invoke(this, e);

    /// <summary>
    ///   The event for the map being loaded
    /// </summary>
    public event MapEventHandler? OnLoad;

    /// <summary>
    ///   Event from the JS notifying of the load
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyLoad(Event e) => OnLoad?.Invoke(this, e);

    /// <summary>
    ///   The event for the map starting to be zoomed
    /// </summary>
    public event MapEventHandler? OnZoomStart;

    /// <summary>
    ///   Event from the JS notifying the change in zoom starting
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyZoomStart(Event e) => OnZoomStart?.Invoke(this, e);

    /// <summary>
    ///   The event for the map starting to be moved
    /// </summary>
    public event MapEventHandler? OnMoveStart;

    /// <summary>
    ///   Event from the JS notifying that a movement has started
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyMoveStart(Event e) => OnMoveStart?.Invoke(this, e);

    /// <summary>
    ///   The event for the map being zoomed
    /// </summary>
    public event MapEventHandler? OnZoom;

    /// <summary>
    ///   Event from the JS notifying they are zooming
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyZoom(Event e) => OnZoom?.Invoke(this, e);

    /// <summary>
    ///   The event for the map being moved
    /// </summary>
    public event MapEventHandler? OnMove;

    /// <summary>
    ///   Event from the JS notifying they are moving
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyMove(Event e) => OnMove?.Invoke(this, e);

    /// <summary>
    ///   The event for the maps zooming coming to an end
    /// </summary>
    public event MapEventHandler? OnZoomEnd;

    /// <summary>
    ///   Event from the JS notifying they are no longer zooming
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyZoomEnd(Event e) => OnZoomEnd?.Invoke(this, e);

    /// <summary>
    ///   The event for the map movement coming to an end
    /// </summary>
    public event MapEventHandler? OnMoveEnd;

    /// <summary>
    ///   Event from the JS notifying they are no longer moving
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyMoveEnd(Event e) => OnMoveEnd?.Invoke(this, e);

    /// <summary>
    ///   The zoom level change event handler
    /// </summary>
    public event MapEventHandler? OnZoomLevelsChange;

    /// <summary>
    ///   Event from the JS notifying the change in zoom levels
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyZoomLevelsChange(Event e) => OnZoomLevelsChange?.Invoke(this, e);

    /// <summary>
    ///   The event for a key being pressed over the map
    /// </summary>
    public event MapEventHandler? OnKeyPress;

    /// <summary>
    ///   Event from the JS notifying that a key has been pressed
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyKeyPress(Event eventArgs) => OnKeyPress?.Invoke(this, eventArgs);

    /// <summary>
    ///   The event for a key being pressed down over the map
    /// </summary>
    public event MapEventHandler? OnKeyDown;

    /// <summary>
    ///   Event from the JS notifying that a key has been pressed down
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyKeyDown(Event eventArgs) => OnKeyDown?.Invoke(this, eventArgs);

    /// <summary>
    ///   The event for a key being released over the map
    /// </summary>
    public event MapEventHandler? OnKeyUp;

    /// <summary>
    ///   Event from the JS notifying that a key has been released
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyKeyUp(Event eventArgs) => OnKeyUp?.Invoke(this, eventArgs);

    /// <summary>
    ///   The event resize handler
    /// </summary>
    public delegate void MapResizeEventHandler(object sender, ResizeEvent e);

    /// <summary>
    ///   The event for the map being resized
    /// </summary>
    public event MapResizeEventHandler? OnResize;

    /// <summary>
    ///   Event from the JS notifying the change in size
    /// </summary>
    /// <param name="e"></param>
    [JSInvokable]
    public void NotifyResize(ResizeEvent e) => OnResize?.Invoke(this, e);



    /// <summary>
    ///   The event for the mouse moving over the map
    /// </summary>
    public event MouseEventHandler? OnMouseMove;

    /// <summary>
    ///   Event from the JS notifying that the mouse is moving
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMouseMove(MouseEvent eventArgs) => OnMouseMove?.Invoke(this, eventArgs);

    /// <summary>
    ///   The event handler for the mouse
    /// </summary>
    public delegate Task MouseEventHandler(Map sender, MouseEvent e);

    /// <summary>
    ///   The event for a pre-click
    /// </summary>
    public event MouseEventHandler? OnPreClick;

    /// <summary>
    ///   Event from the JS notifying that a pre-click has happened
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyPreClick(MouseEvent eventArgs) => OnPreClick?.Invoke(this, eventArgs);

    /// <summary>
    ///   The event handler for clicking
    /// </summary>
    public event MouseEventHandler? OnClick;

    /// <summary>
    ///   Event from the JS notifying that a click has happened
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyClick(MouseEvent eventArgs) => OnClick?.Invoke(this, eventArgs);

    /// <summary>
    ///  The event handler for a double click
    /// </summary>
    public event MouseEventHandler? OnDblClick;

    /// <summary>
    ///   Event from the JS notifying that a double click has happened
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyDblClick(MouseEvent eventArgs) => OnDblClick?.Invoke(this, eventArgs);

    /// <summary>
    ///   The event handler for pressing a button down on a mouse
    /// </summary>
    public event MouseEventHandler? OnMouseDown;

    /// <summary>
    ///   Event from the JS notifying that a mouse button has been pressed down
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMouseDown(MouseEvent eventArgs) => OnMouseDown?.Invoke(this, eventArgs);

    /// <summary>
    ///   The event for when you release the button on a mouse
    /// </summary>
    public event MouseEventHandler? OnMouseUp;

    /// <summary>
    ///   Event from the JS notifying that a mouse button has been released
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMouseUp(MouseEvent eventArgs) => OnMouseUp?.Invoke(this, eventArgs);

    /// <summary>
    ///   The event for when you mouse over the map
    /// </summary>
    public event MouseEventHandler? OnMouseOver;

    /// <summary>
    ///   Event from the JS notifying that the user is mousing over
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMouseOver(MouseEvent eventArgs) => OnMouseOver?.Invoke(this, eventArgs);

    /// <summary>
    ///   The event for when your mouse leaves the map
    /// </summary>
    public event MouseEventHandler? OnMouseOut;

    /// <summary>
    ///   Event from the JS notifying that the user is no longer mousing over
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyMouseOut(MouseEvent eventArgs) => OnMouseOut?.Invoke(this, eventArgs);

    /// <summary>
    ///   The event handler for when the context menu gets opened
    /// </summary>
    public event MouseEventHandler? OnContextMenu;

    /// <summary>
    ///   Event from the JS notifying that the context menu has been opened
    /// </summary>
    /// <param name="eventArgs"></param>
    [JSInvokable]
    public void NotifyContextMenu(MouseEvent eventArgs) => OnContextMenu?.Invoke(this, eventArgs);
    #endregion events
}