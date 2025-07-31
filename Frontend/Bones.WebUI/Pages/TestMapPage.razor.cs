using System.Drawing;
using Bones.WebUI.Leaflet;
using Bones.WebUI.Leaflet.Data;
using Bones.WebUI.Leaflet.Models;
using Bones.WebUI.Leaflet.Models.Events;
using Microsoft.JSInterop;

namespace Bones.WebUI.Pages;

/// <summary>
///   A page made for testing maps
/// </summary>
public partial class TestMapPage(IJSRuntime JsRuntime) : ComponentBase
{
    private readonly LatLon _startAt = new(31.887f, -100.360f, 7);

    private readonly Polygon _colorado = new()
    {
        Shape = [
            [new(36.9990f, -109.0452f), new(41.0007f, -109.0500f), new(41.0024f, -102.0516f), new(36.9931f, -102.0421f)]
        ],
        Fill = true,
        FillColor = Color.Blue,
        Popup = new()
        {
            Content = "This is Colorado"
        }
    };

    private readonly Circle _roswell = new()
    {
        Position = new(33.394181f, -104.522660f),
        Radius = 10000
    };

    private LatLon _markerAt = new(31.441500f, -100.465396f);

    private Map? _map;

    /// <summary>
    ///   Its the map
    /// </summary>
    protected Map Map
    {
        get
        {
            _map ??= new(JsRuntime)
            {
                Center = _startAt,
                Zoom = 4.8f
            };

            return _map;
        }
    }

    private DrawHandler? _drawHandler;

    /// <summary>
    ///   It handles drawing on the map
    /// </summary>
    protected DrawHandler DrawHandler
    {
        get
        {
            if (_drawHandler == null)
            {
                _drawHandler = new(Map, JsRuntime);
            }

            return _drawHandler;
        }
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Map.OnInitialized += () =>
        {
            Map.AddLayer(_colorado);

            Map.AddLayer(_roswell);

            Marker marker = new(_markerAt)
            {
                Draggable = true,
                Popup = new() { Content = $"I am at {_markerAt.Lat:0.0000}° lat, {_markerAt.Lon:0.0000}° lon" },
                Tooltip = new() { Content = "Click and drag to move me" }
            };

            marker.OnMove += OnDrag;
            marker.OnMoveEnd += OnDragEnd;

            Map.AddLayer(marker);
        };
    }

    private void OnDrag(Marker marker, DragEvent evt)
    {
        if (evt.LatLon != null)
        {
            _markerAt = evt.LatLon;
            StateHasChanged();
        }
    }

    private async void OnDragEnd(Marker marker, Event e)
    {
        marker.Position = _markerAt;
        if (marker.Popup == null)
        {
            marker.Popup = new()
            {
                Content = $"I am now at {_markerAt.Lat:0.0000}° lat, {_markerAt.Lon:0.0000}° lon"
            };
        }
        else
        {
            marker.Popup.Content = $"I am now at {_markerAt.Lat:0.0000}° lat, {_markerAt.Lon:0.0000}° lon";
        }

        await LeafletInterops.UpdatePopupContentAsync(JsRuntime, Map.Id, marker);
    }
}