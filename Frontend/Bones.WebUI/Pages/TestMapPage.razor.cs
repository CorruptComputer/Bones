using System.Drawing;
using Bones.WebUI.Leaflet;
using Bones.WebUI.Leaflet.Data;
using Bones.WebUI.Leaflet.Models;
using Bones.WebUI.Leaflet.Models.Events;
using Microsoft.AspNetCore.Components;

namespace Bones.WebUI.Pages;

/// <summary>
///   A page made for testing maps
/// </summary>
public partial class TestMapPage : ComponentBase
{
    private readonly LatLon _startAt = new(31.887f, -100.360f, 7);
    private LatLon _markerAt = new(31.887f, -100.360f);

    private Map? _map;

    /// <summary>
    ///   Its the map
    /// </summary>
    protected Map Map
    {
        get
        {
            if (_map == null)
            {
                _map = new(JsRuntime)
                {
                    Center = _startAt,
                    Zoom = 4.8f
                };
            }

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
            Map.AddLayer(new TileLayer
            {
                UrlTemplate = "https://a.tile.openstreetmap.org/{z}/{x}/{y}.png",
                Attribution = "&copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors",
            });

            Map.AddLayer(new Polygon
            {
                Shape = [
                    [new(37f, -109.05f), new(41f, -109.03f), new(41f, -102.05f), new(37f, -102.04f)]
                ],
                Fill = true,
                FillColor = Color.Blue,
                Popup = new()
                {
                    Content = "This is Colorado"
                }
            });

            Map.AddLayer(new Circle
            {
                Position = new(33.394181f, -104.522660f),
                Radius = 10000
            });

            Marker marker = new(_markerAt)
            {
                Draggable = true,
                Title = "Marker 1",
                Popup = new() { Content = $"I am at {_markerAt.Lat:0.00}° lat, {_markerAt.Lon:0.00}° lon" },
                Tooltip = new() { Content = "Click and drag to move me" }
            };

            marker.OnMove += OnDrag;
            marker.OnMoveEnd += OnDragEnd;

            Map.AddLayer(marker);

            Map.OnInitialized += () =>
            {
                Map.AddLayer(new TileLayer
                {
                    UrlTemplate = "https://a.tile.openstreetmap.org/{z}/{x}/{y}.png",
                    Attribution = "&copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors",
                });
            };
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
                Content = $"I am now at {_markerAt.Lat:0.00}° lat, {_markerAt.Lon:0.00}° lon"
            };
        }
        else
        {
            marker.Popup.Content = $"I am now at {_markerAt.Lat:0.00}° lat, {_markerAt.Lon:0.00}° lon";
        }

        await LeafletInterops.UpdatePopupContentAsync(JsRuntime, Map.Id, marker);
    }
}