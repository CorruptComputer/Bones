using System.Collections.Concurrent;
using System.Drawing;
using Bones.WebUI.Leaflet.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Rectangle = Bones.WebUI.Leaflet.Models.Rectangle;

namespace Bones.WebUI.Leaflet;

/// <summary>
///   Leaflet interoperability
/// </summary>
public static class LeafletInterops
{
    private static ConcurrentDictionary<string, (IDisposable, string, Layer)> LayerReferences { get; } = new();

    private const string _baseObjectContainer = "window.leafletInterop";

    /// <summary>
    ///   Creates the map on the page
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="map"></param>
    public static async Task CreateAsync(IJSRuntime jsRuntime, Map map)
    {
        await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.create", map, DotNetObjectReference.Create(map));
    }

    /// <summary>
    ///   Adds a layer to the specified map on the page
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="layer"></param>
    /// <exception cref="NotImplementedException"></exception>
    public static async Task AddLayerAsync(IJSRuntime jsRuntime, string mapId, Layer layer)
    {
        switch (layer)
        {
            case TileLayer tileLayer:
                await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.addTilelayer", mapId, tileLayer,
                    CreateLayerReference(mapId, tileLayer));
                break;
            case MbTilesLayer mbTilesLayer:
                await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.addMbTilesLayer", mapId, mbTilesLayer,
                    CreateLayerReference(mapId, mbTilesLayer));
                break;
            case ShapefileLayer shapefileLayer:
                await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.addShapefileLayer", mapId, shapefileLayer,
                    CreateLayerReference(mapId, shapefileLayer));
                break;
            case Marker marker:
                await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.addMarker", mapId, marker,
                    CreateLayerReference(mapId, marker));
                break;
            case Rectangle rectangle:
                await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.addRectangle", mapId, rectangle,
                    CreateLayerReference(mapId, rectangle));
                break;
            case Circle circle:
                await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.addCircle", mapId, circle,
                    CreateLayerReference(mapId, circle));
                break;
            case Polygon polygon:
                await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.addPolygon", mapId, polygon,
                    CreateLayerReference(mapId, polygon));
                break;
            case Polyline polyline:
                await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.addPolyline", mapId, polyline,
                    CreateLayerReference(mapId, polyline));
                break;
            case ImageLayer image:
                await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.addImageLayer", mapId, image,
                    CreateLayerReference(mapId, image));
                break;
            case GeoJsonDataLayer geo:
                await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.addGeoJsonLayer", mapId, geo,
                    CreateLayerReference(mapId, geo));
                break;
            default:
                throw new NotImplementedException($"The layer {typeof(Layer).Name} has not been implemented.");
        }
    }

    /// <summary>
    ///   Removes a layer from the specified map on the page
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="layerId"></param>
    public static async Task RemoveLayerAsync(IJSRuntime jsRuntime, string mapId, string layerId)
    {
        await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.removeLayer", mapId, layerId);
        DisposeLayerReference(layerId);
    }

    /// <summary>
    ///   Updates the popup content on the page
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="layer"></param>
    public static async Task UpdatePopupContentAsync(IJSRuntime jsRuntime, string mapId, Layer layer)
    {
        await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.updatePopupContent", mapId, layer.Id, layer.Popup?.Content);
    }

    /// <summary>
    ///   Updates the tooltip content on the page
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="layer"></param>
    public static async Task UpdateTooltipContentAsync(IJSRuntime jsRuntime, string mapId, Layer layer)
    {
        await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.updateTooltipContent", mapId, layer.Id, layer.Tooltip?.Content);
    }

    /// <summary>
    ///   Updates the shape on the page
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="layer"></param>
    /// <exception cref="NotImplementedException"></exception>
    public static async Task UpdateShapeAsync(IJSRuntime jsRuntime, string mapId, Layer layer)
    {
        switch (layer)
        {
            case Rectangle rectangle:
                await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.updateRectangle", mapId, rectangle);
                break;
            case Circle circle:
                await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.updateCircle", mapId, circle);
                break;
            case Polygon polygon:
                await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.updatePolygon", mapId, polygon);
                break;
            case Polyline polyline:
                await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.updatePolyline", mapId, polyline);
                break;
            default:
                throw new NotImplementedException($"The layer {nameof(Layer)} has not been implemented.");
        }
    }

    /// <summary>
    ///   Fits the map to the given bounds
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="corner1"></param>
    /// <param name="corner2"></param>
    /// <param name="padding"></param>
    /// <param name="maxZoom"></param>
    public static async Task FitBoundsAsync(IJSRuntime jsRuntime, string mapId, PointF corner1, PointF corner2,
        PointF? padding, float? maxZoom)
    {
        await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.fitBounds", mapId, corner1, corner2, padding, maxZoom);
    }

    /// <summary>
    ///   Pans to the map to the specified location
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="position"></param>
    /// <param name="animate"></param>
    /// <param name="duration"></param>
    /// <param name="easeLinearity"></param>
    /// <param name="noMoveStart"></param>
    public static async Task PanToAsync(IJSRuntime jsRuntime, string mapId, PointF position, bool animate, float duration,
        float easeLinearity, bool noMoveStart)
    {
        await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.panTo", mapId, position, animate, duration, easeLinearity, noMoveStart);
    }

    /// <summary>
    ///   Gets the center of the specified map
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <returns></returns>
    public static async Task<LatLon> GetCenterAsync(IJSRuntime jsRuntime, string mapId)
    {
        return await jsRuntime.InvokeAsync<LatLon>($"{_baseObjectContainer}.getCenter", mapId);
    }

    /// <summary>
    ///   Gets the current zoom level for the specified map
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <returns></returns>
    public static async Task<float> GetZoomAsync(IJSRuntime jsRuntime, string mapId)
    {
        return await jsRuntime.InvokeAsync<float>($"{_baseObjectContainer}.getZoom", mapId);
    }

    /// <summary>
    ///   Zooms in on the specified map
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="e"></param>
    public static async Task ZoomInAsync(IJSRuntime jsRuntime, string mapId, MouseEventArgs e)
    {
        await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.zoomIn", mapId, e);
    }

    /// <summary>
    ///   Zooms out on the specified map
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="mapId"></param>
    /// <param name="e"></param>
    public static async Task ZoomOutAsync(IJSRuntime jsRuntime, string mapId, MouseEventArgs e)
    {
        await jsRuntime.InvokeVoidAsync($"{_baseObjectContainer}.zoomOut", mapId, e);
    }


    private static DotNetObjectReference<T> CreateLayerReference<T>(string mapId, T layer) where T : Layer
    {
        DotNetObjectReference<T>? result = DotNetObjectReference.Create(layer);
        LayerReferences.TryAdd(layer.Id, (result, mapId, layer));
        return result;
    }

    private static void DisposeLayerReference(string layerId)
    {
        if (LayerReferences.TryRemove(layerId, out (IDisposable, string, Layer) value))
        {
            value.Item1.Dispose();
        }
    }
}